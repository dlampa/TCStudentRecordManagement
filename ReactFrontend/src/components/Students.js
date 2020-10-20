import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback, Alert, Modal, ModalBody, ModalHeader, ModalFooter } from 'reactstrap';
import StudentsTable from './StudentsTable';
import AppNavbar from './Navbar';

import { ax } from '../js';

class Students extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            activeCohortID: null,
            status: null,
            studentData: null,
            cohortData: null,
            dialogs: { dialogAddStudent: false, dialogModStudent: false },
            newStudent: {
                firstname: null, firstnameInvalid: true,
                lastname: null, lastnameInvalid: true,
                email: null, emailInvalid: true,
                bearTracksID: null,
                invalidSub: true
            },
            activeStudent: {
                studentID: null,
                firstname: null, firstnameInvalid: true,
                lastname: null, lastnameInvalid: true,
                email: null, emailInvalid: true,
                bearTracksID: null,
                invalidSub: true
            }
        };
    }

    // Listen for resize events, then update the isMobile state depending on screen width
    // Ref: https://goshakkk.name/different-mobile-desktop-tablet-layouts-react/
    componentDidMount() {
        // this.setState({ currentDate: DateTime.local().toISODate() });
        // this.setState({ newTask: { duration: 0, durationInvalid: false, startTime: "00:00", endTime: "00:00" } });

        window.addEventListener('resize', this.handleWindowSizeChange);
        this.handleWindowSizeChange();

        // Retrieve a list of Cohorts
        this.retrieveCohorts();



        // Retrieve the list of Students
        this.retrieveStudents();

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
            this.retrieveStudents();
        }

    }

    moveCohort = (direction) => {
        // Find position based on value
        // Ref: https://stackoverflow.com/a/15998003/12802214
        const thisCohortIDPos = this.state.cohortData.findIndex(x => x.cohortID == this.state.activeCohortID);
        if (thisCohortIDPos - 1 >= 0 && direction == "rev") this.setState({ activeCohortID: this.state.cohortData[thisCohortIDPos - 1].cohortID });
        if (thisCohortIDPos + 1 <= this.state.cohortData.length - 1 && direction == "fwd") this.setState({ activeCohortID: this.state.cohortData[thisCohortIDPos + 1].cohortID });
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
            case "addStudent":
                this.setState((state) => state.dialogs.dialogAddStudent = !state.dialogs.dialogAddStudent);
                break;
            case "modStudent":
                this.setState((state) => state.dialogs.dialogModStudent = !state.dialogs.dialogModStudent);
                break;
            default:
                // For future development
                console.log(dialog);
        }
    }

    clearStudentInputs = async (studentObject) => {
        await this.setState({
            [studentObject]: {
                firstname: null, firstnameInvalid: true,
                lastname: null, lastnameInvalid: true,
                email: null, emailInvalid: true,
                bearTracksID: null,
                invalidSub: true
            }
        });
    }

    addStudent = async () => {
        if (this.props?.auth !== undefined && !this.state.newStudent.invalidSub) {
            const response = await ax(process.env.REACT_APP_APIURL_STUDENTS_ADD, {
                method: 'put',
                params: {
                    firstname: this.state.newStudent.firstname,
                    lastname: this.state.newStudent.lastname,
                    email: this.state.newStudent.email,
                    active: true,
                    cohortID: Number(this.state.activeCohortID),
                    bearTracksID: this.state.newStudent.bearTracksID
                }
            }, this.props.auth.tokenID);

            console.log(response);

            if (response.status === 200) {
                // Reset error message output
                await this.setState({ status: [] });

                // Hide the dialog
                this.toggleDialog("addStudent");

                // Reset input fields
                await this.clearStudentInputs("newStudent");

            } else {
                // Commit error messages to state
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }

        }
    }

    modifyStudent = async () => {
        if (this.props?.auth !== undefined && !this.state.activeStudent.invalidSub) {
            const response = await ax(process.env.REACT_APP_APIURL_STUDENTS_MODIFY, {
                method: 'put',
                data: {
                    StudentID: Number(this.state.activeStudent.studentID),
                    CohortID: Number(this.state.activeCohortID),
                    Firstname: this.state.activeStudent.firstname,
                    Lastname: this.state.activeStudent.lastname,
                    Email: this.state.activeStudent.email,
                    Active: true,
                    BearTracksID: this.state.activeStudent.bearTracksID
                }
            }, this.props.auth.tokenID);

            console.log(response);

            if (response.status === 200) {
                // Reset error message output
                await this.setState({ status: [] });

                // Hide the dialog
                this.toggleDialog("modStudent");

                // Reset input fields
                await this.clearStudentInputs("activeStudent");

                // Refresh student table
                this.retrieveStudents();

            } else {
                // Commit error messages to state
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }

        }
    }





    retrieveCohorts = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_COHORTS_LISTACTIVE, {}, this.props.auth.tokenID);

            if (response?.data !== undefined) {
                await this.setState({ cohortData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveStudents = async () => {
        if (this.props?.auth !== undefined) {
            const response = await ax(process.env.REACT_APP_APIURL_STUDENTS_DETAILS, {
                params: {
                    cohortID: Number(this.state.activeCohortID)
                }
            }, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                await this.setState({ studentData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    deleteStudent = async (studentRecord) => {
        // This method is only usable by superAdmin
        if (this.props?.auth !== undefined) {
            const response = await ax(process.env.REACT_APP_APIURL_STUDENTS_DELETE, {
                method: 'delete',
                params: {
                    id: studentRecord.studentID
                }
            }, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                this.setState({ APIMessage: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    changeStudentActiveState = async (studentRecord) => {
        if (this.props?.auth !== undefined) {
            const response = await ax(process.env.REACT_APP_APIURL_STUDENTS_ACTIVATE, {
                method: 'put',
                params: {
                    id: Number(studentRecord.studentID),
                    state: !studentRecord.user.active
                }
            }, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                this.setState({ APIMessage: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    tableFunctions = async (callerID, studentRecord) => {
        switch (callerID) {
            case ("studentEdit"):
                const activeStudent = this.state.activeStudent;
                activeStudent.studentID = studentRecord.studentID;
                activeStudent.firstname = studentRecord.user.firstname;
                activeStudent.firstnameInvalid = false;
                activeStudent.lastname = studentRecord.user.lastname;
                activeStudent.lastnameInvalid = false;
                activeStudent.email = studentRecord.user.email;
                activeStudent.emailInvalid= false;
                activeStudent.bearTracksID = studentRecord.beartTracksID;
                await this.setState({ activeStudent: activeStudent});
                this.toggleDialog("modStudent");
                this.retrieveStudents();
                break;
            case ("studentState"):
                await this.changeStudentActiveState(studentRecord);
                this.retrieveStudents();
                break;
            case ("studentDelete"):
                await this.setState({ activeStudentID: studentRecord.studentID });
                await this.deleteTimesheetRecord();
                this.retrieveStudents();
                break;
            default:
                break;
        }
    }

    render() {

        // Redirect to first page if login data is missing
        if (this.props?.auth?.tokenID === undefined) {
            this.props.history.push(process.env.PUBLIC_URL);
        } else if (this.props.auth.rights === "Student") {
            this.props.history.push(process.env.PUBLIC_URL);
        }

        return (
            <main>
                <header>
                    <AppNavbar mobile={this.state.windowState === "mobile"} />
                </header>
                <article id="studentRecordManagement">
                    <h2 className="titleDate">
                        Student record management
                    </h2>

                    {/* 
                    Use of onClick events with parameters
                    Ref: https://upmostly.com/tutorials/pass-a-parameter-through-onclick-in-react 
                
                */}
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

                    <section id="inputAddStudents">
                        <Button id="addStudent" name="addStudent" color="info" onClick={() => this.toggleDialog("addStudent")} title="Add student to cohort"><i className="fas fa-user-plus"></i></Button>
                        <Button id="addStudentsBulk" name="addStudentsBulk" onClick={() => this.toggleDialog("addStudent")} title="Bulk add students to cohort"><i className="fas fa-users"></i></Button>
                        <Modal id="dialogAddStudent" isOpen={this.state.dialogs.dialogAddStudent} toggle={() => this.toggleDialog("addStudent")}>
                            <ModalHeader toggle={() => this.toggleDialog("addStudent")}>Add Student</ModalHeader>
                            <ModalBody>
                                <h4>{this.state?.cohortData?.find(x => x.cohortID == this.state.activeCohortID)?.name}</h4>
                                <FormGroup>
                                    <FormGroup>
                                        <Label for="studentFirstname">First name:</Label>
                                        <Input name="studentFirstname" id="studentFirstname" value={this.state.newStudent.firstname ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.newStudent.firstnameInvalid} />
                                        <FormFeedback>Firstname cannot be empty or consist of only whitespace.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentLastname">Last name:</Label>
                                        <Input name="studentLastname" id="studentLastname" value={this.state.newStudent.lastname ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.newStudent.lastnameInvalid} />
                                        <FormFeedback>Lastname cannot be empty or consist of only whitespace.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentEmail">Email:</Label>
                                        <Input type="email" name="studentEmail" id="studentEmail" value={this.state.newStudent.email ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.newStudent.emailInvalid} />
                                        <FormFeedback>Email address must be unique - a user with this email address already exists.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentBearTracksID">BearTracksID (optional):</Label>
                                        <Input name="studentBearTracksID" id="studentBearTracksID" value={this.state.newStudent.bearTracksID} onChange={(event) => this.inputChange(event)} />
                                    </FormGroup>
                                </FormGroup>
                                <Alert color="danger" hidden={!(this.state.status?.length > 0)}>
                                    <ul>
                                        {Array.isArray(this.state?.status) ? this.state.status.map((status, index) => (<li key={index}>{status}</li>)) : null}
                                    </ul>
                                    Please correct the error(s) and try again.
                                </Alert>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="info" onClick={() => { this.addStudent(); }}>Add Student</Button>{' '}
                                <Button onClick={() => { this.toggleDialog("addStudent") ; this.clearStudentInputs("addStudent") }}>Cancel</Button>
                            </ModalFooter>
                        </Modal>
                    </section>

                    <section id="inputModStudent">
                            <Modal id="dialogModStudent" isOpen={this.state.dialogs.dialogModStudent} toggle={() => this.toggleDialog("modStudent")}>
                            <ModalHeader toggle={() => this.toggleDialog("modStudent")}>Modify Student record</ModalHeader>
                            <ModalBody>
                                <FormGroup>
                                    <FormGroup>
                                        <Label for="studentFirstname">Firstname</Label>
                                        <Input name="studentFirstname" id="studentModFirstname" value={this.state.activeStudent.firstname ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.activeStudent.firstnameInvalid} />
                                        <FormFeedback>Firstname cannot be empty or consist of only whitespace.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentLastname">Lastname</Label>
                                        <Input name="studentLastname" id="studentModLastname" value={this.state.activeStudent.lastname ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.activeStudent.lastnameInvalid} />
                                        <FormFeedback>Lastname cannot be empty or consist of only whitespace.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentEmail">Email</Label>
                                        <Input type="email" name="studentEmail" id="studentModEmail" value={this.state.activeStudent.email ?? ""} onChange={(event) => this.inputChange(event)} invalid={this.state.activeStudent.emailInvalid} />
                                        <FormFeedback>Email address must be unique - a user with this email address already exists.</FormFeedback>
                                    </FormGroup>

                                    <FormGroup>
                                        <Label for="studentBearTracksID">BearTracksID (optional)</Label>
                                        <Input name="studentBearTracksID" id="studentModBearTracksID" value={this.state.activeStudent.bearTracksID} onChange={(event) => this.inputChange(event)} />
                                    </FormGroup>
                                </FormGroup>
                                <Alert color="danger" hidden={!(this.state.status?.length > 0)}>
                                    <ul>
                                        {Array.isArray(this.state?.status) ? this.state.status.map((status, index) => (<li key={index}>{status}</li>)) : null}
                                    </ul>
                                    Please correct the error(s) and try again.
                                </Alert>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="info" onClick={() => { this.modifyStudent(); }}>Modify Student</Button>{' '}
                                <Button onClick={() => { this.toggleDialog("modStudent"); this.clearStudentInputs("activeStudent") }}>Cancel</Button>
                            </ModalFooter>
                        </Modal>
                    </section>


                    <section id="tableStudents">
                        <h3>Users</h3>

                        <StudentsTable studentData={this.state.studentData} mobile={this.state.windowState === "mobile"} role={this.props.auth?.rights} func={this.tableFunctions} />
                    </section>
                </article>
            </main>
        );
    }
}



export default withRouter(
    connect(
        (state) => {
            return { auth: state.auth }
        }
    )(Students));

