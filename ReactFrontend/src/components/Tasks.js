import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback, Alert, Modal, ModalBody, ModalHeader, ModalFooter } from 'reactstrap';


import AppNavbar from './AppNavbar';
import TasksTable from './TasksTable';

import { ax } from '../js';

class Tasks extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            dialogs: { dialogCreateTask: false, dialogModStudent: false },
            cohortData: []
        };
    }

    // Listen for resize events, then update the isMobile state depending on screen width
    // Ref: https://goshakkk.name/different-mobile-desktop-tablet-layouts-react/
    componentDidMount() {
        window.addEventListener('resize', this.handleWindowSizeChange);
        this.handleWindowSizeChange();

        // Retrieve a list of Cohorts
        this.retrieveCohorts();
        
        // Retrieve a list of TaskTypes
        this.retrieveTaskTypes();

        // Retrieve a list of Units
        this.retrieveUnits();

        // Retrieve a list of Tasks
        this.retrieveTasks();
        
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.handleWindowSizeChange);
    }

    handleWindowSizeChange = () => {
        // Modify state which needs to change on window resize event
        const _windowState = window.innerWidth < 500 ? "mobile" : window.innerWidth < 1000 ? "tablet" : "desktop";

        this.setState({
            windowState: _windowState,
        });
    };

    componentDidUpdate = (prevProps, prevState) => {

        // Select first cohort from the list after cohortData load
        if (this.state.cohortData !== prevState.cohortData) {
            this.setState({ activeCohortID: Number(this.state?.cohortData[0].cohortID) || 0 });
        }

        // Update student lists when active cohortID changes
        if (this.state.activeCohortID !== prevState.activeCohortID) {
            this.retrieveTasks();
        }

    }

    moveCohort = (direction) => {
        // Find position based on value
        // Ref: https://stackoverflow.com/a/15998003/12802214
        const thisCohortIDPos = this.state.cohortData.findIndex(x => x.cohortID == this.state.activeCohortID);
        if (thisCohortIDPos - 1 >= 0 && direction == "rev") this.setState({ activeCohortID: this.state.cohortData[thisCohortIDPos - 1].cohortID });
        if (thisCohortIDPos + 1 <= this.state.cohortData.length - 1 && direction == "fwd") this.setState({ activeCohortID: this.state.cohortData[thisCohortIDPos + 1].cohortID });
    }

    retrieveCohorts = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_COHORTS_LISTACTIVE, {}, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                this.cohoortData = response.data;
                await this.setState({ cohortData: response.data }, () => true);
                
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }
    
    
    retrieveTaskTypes = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_TASKTYPES_LIST, {}, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                await this.setState({ taskTypeData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveUnits = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_UNITS_LIST, {}, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                await this.setState({ unitsData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveTasks = async () => {
        if (this.props?.auth !== undefined) {
            console.log(this.state.cohortData);
            let test = this.state.cohortData[0]; // (x => x.cohortID == this.activeCohortID);

            const response = await ax(process.env.REACT_APP_APIURL_TASKS_GET, {
                params: {
                    cohortID: Number(this.state.activeCohortID),
                    startDate: ""
                }
            }, this.props.auth.tokenID);

            if (response?.data !== undefined) {
                
                this.setState({ tasksData: this.response.data } );
                this.setState({ status: [] });
            } else {
                this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }


    createTask = async () => {
        
    }
    
    inputChange = async (event) => {
        let newStudent = this.state.newStudent;
        let activeStudent = this.state.activeStudent;
        switch (event?.target?.id) {
            case "selectCohort":
                await this.setState({ activeCohortID: Number(event.target.value) });
                break;
            case "studentFirstname":
                newStudent.firstname = event.target.value;
                newStudent.firstnameInvalid = (newStudent.firstname.trim().length == 0);
                newStudent.invalidSub = newStudent.firstnameInvalid || newStudent.lastnameInvalid || newStudent.emailInvalid;
                await this.setState({ newStudent });
                break;
            case "studentLastname":
                newStudent.lastname = event.target.value;
                newStudent.lastnameInvalid = (newStudent.lastname.trim().length == 0);
                newStudent.invalidSub = newStudent.firstnameInvalid || newStudent.lastnameInvalid || newStudent.emailInvalid;
                await this.setState({ newStudent });
                break;
            case "studentEmail":
                newStudent.email = event.target.value;
                newStudent.emailInvalid = (false);
                newStudent.invalidSub = newStudent.firstnameInvalid || newStudent.lastnameInvalid || newStudent.emailInvalid;
                await this.setState({ newStudent });
                break;
            case "studentBearTracksID":
                newStudent.bearTracksID = event.target.value;
                newStudent.invalidSub = newStudent.firstnameInvalid || newStudent.lastnameInvalid || newStudent.emailInvalid;
                await this.setState({ newStudent });
                break;
            case "studentModFirstname":
                activeStudent.firstname = event.target.value;
                activeStudent.firstnameInvalid = (activeStudent.firstname.trim().length == 0);
                activeStudent.invalidSub = activeStudent.firstnameInvalid || activeStudent.lastnameInvalid || activeStudent.emailInvalid;
                await this.setState({ activeStudent });
                break;
            case "studentModLastname":
                activeStudent.lastname = event.target.value;
                activeStudent.lastnameInvalid = (activeStudent.lastname.trim().length == 0);
                activeStudent.invalidSub = activeStudent.firstnameInvalid || activeStudent.lastnameInvalid || activeStudent.emailInvalid;
                await this.setState({ activeStudent });
                break;
            case "studentModEmail":
                activeStudent.email = event.target.value;
                activeStudent.emailInvalid = (false);
                activeStudent.invalidSub = activeStudent.firstnameInvalid || activeStudent.lastnameInvalid || activeStudent.emailInvalid;
                await this.setState({ activeStudent });
                break;
            case "studentModBearTracksID":
                activeStudent.bearTracksID = event.target.value;
                activeStudent.invalidSub = activeStudent.firstnameInvalid || activeStudent.lastnameInvalid || activeStudent.emailInvalid;
                await this.setState({ activeStudent });
                break;
            default:
                console.log(event.target?.id);
                break;
        }
    }

    toggleDialog = async (dialog) => {
        switch (dialog) {
            case "createTask":
                this.setState((state) => state.dialogs.dialogCreateTask = !state.dialogs.dialogCreateTask);
                break;
            case "modStudent":
                this.setState((state) => state.dialogs.dialogModStudent = !state.dialogs.dialogModStudent);
                break;
            default:
                // For future development
                console.log(dialog);
        }
    }



    render() {
        // Redirect to first page if login data is missing
        if (this.props?.auth?.tokenID === null || this.props.auth === null || this.props?.auth?.rights === "Student") {
            this.props.history.push(process.env.PUBLIC_URL);
        } else

        return (
            <article>
                <header>
                    <AppNavbar mobile={this.state.windowState === "mobile"} />
                </header>
                <main id="taskManagement">
                    <h2 className="title">
                        Tasks
                    </h2>

                    <FormGroup id="cohortPicker">

                        <InputGroup>
                            <InputGroupAddon addonType="prepend">
                                <Button id="revCohort" onClick={() => this.moveCohort("rev")}><i className="fas fa-angle-left" /></Button>
                            </InputGroupAddon>

                            <Input type="select" name="selectCohort" id="selectCohort" value={this.state.activeCohortID} onChange={(event) => this.inputChange(event)}>
                                {/* Retrieve the list of tasks for the Student's cohort, sorted reverse by endDate */}
                                {this.state.cohortData?.map((cohort, index) => (<option key={index} value={cohort.cohortID}>{cohort.name}</option>))}
                            </Input>

                            <InputGroupAddon addonType="append">
                                <Button id="fwdCohort" disabled={false} onClick={() => this.moveCohort("fwd")}><i className="fas fa-angle-right" /></Button>
                            </InputGroupAddon>
                        </InputGroup>
                    </FormGroup>

                  

                    <section id="tableTasks">
                        <h3>Users</h3>

                      
                    </section>
                </main>
            </article>
            
        );
    }

}

export default withRouter(connect((state) => {
    return { auth: state.auth }
})(Tasks));