import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback, Alert, Modal, ModalBody, ModalHeader, ModalFooter } from 'reactstrap';
import { DateTime } from 'luxon';

import AppNavbar from './AppNavbar';
import AttendanceTable from './AttendanceTable';

import { ax } from '../js';

class Attendance extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            activeCohortID: null,
            activeDate: null,
            status: null,
            studentData: null,
            attendanceData: null,
            attendanceStatesData: null,
            cohortData: null,
            
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

        this.retrieveAttendanceStates();



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
    
    moveDate = (interval) => {
        if (this.state.currentDate !== DateTime.local.toISODate) {
            this.setState({ currentDate: DateTime.fromISO(this.state.currentDate).plus({ days: interval }).toISODate() });
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
                await this.setState({ cohortData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveAttendanceStates = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_ATTENDANCESTATES_LIST, {}, this.props.auth.tokenID);

            if (response?.data !== undefined) {
                await this.setState({ attendanceStatesData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveAttendanceRecords = async () => {
        if (this.props?.auth !== undefined) {

            const response = await ax(process.env.REACT_APP_APIURL_ATTENDANCESTATE_, {}, this.props.auth.tokenID);

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

    tableFunctions = async (callerID, studentRecord) => {
        switch (callerID) {
            case ("commentEdit"):
                const activeStudent = this.state.activeStudent;
                activeStudent.studentID = studentRecord.studentID;
                activeStudent.firstname = studentRecord.user.firstname;
                activeStudent.lastname = studentRecord.user.lastname;
                activeStudent.email = studentRecord.user.email;
                activeStudent.bearTracksID = studentRecord.beartTracksID;
                await this.setState({ activeStudent: activeStudent });
                this.toggleDialog("modComment");
                break;
            case ("studentState"):
                await this.changeStudentActiveState(studentRecord);
                break;
            case ("studentDelete"):
                await this.setState({ activeStudentID: studentRecord.studentID });
                await this.deleteTimesheetRecord();
                this.retrieveTimesheets();
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
                        Attendance
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

                    <FormGroup id="datePicker">
                        <Label for="selectedDate">Date</Label>
                        <InputGroup>
                            <InputGroupAddon addonType="prepend">
                                <Button id="reverseDate" onClick={() => this.moveDate(-1)}><i className="fas fa-angle-left" /></Button>
                            </InputGroupAddon>
                            <Input type="date" name="date" id="selectedDate" max={DateTime.local().toISODate()} value={this.state.currentDate} onChange={(event) => this.inputChange(event)} />
                            <InputGroupAddon addonType="append">
                                <Button id="forwardDate" disabled={this.state.currentDate === DateTime.local().toISODate()} onClick={() => this.moveDate(1)}><i className="fas fa-angle-right" /></Button>
                            </InputGroupAddon>
                        </InputGroup>
                    </FormGroup>
                   

                  

                    <section id="tableStudents">
                        <h3>Users</h3>

                        <AttendanceTable studentData={this.state.studentData} attendanceStatesData={this.state.attendanceStatesData} mobile={this.state.windowState === "mobile"} role={this.props.auth?.rights} func={this.tableFunctions} />
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
    )(Attendance));


