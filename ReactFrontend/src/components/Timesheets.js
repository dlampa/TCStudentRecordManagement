import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { DateTime, Interval, Duration } from 'luxon';
import { Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback, Alert } from 'reactstrap';
import TimesheetTable from './TimesheetTable';
import AppNavbar from './Navbar';

import { ax } from '../js';




class Timesheets extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            currentDate: DateTime.local().toISODate(),
            newTask: { duration: 0, durationInvalid: false, startTime: "00:00", endTime: "00:00" },
            status: null,
            timesheetData: null,
            taskData: null
        };
    }

    // Listen for resize events, then update the isMobile state depending on screen width
    // Ref: https://goshakkk.name/different-mobile-desktop-tablet-layouts-react/
    componentDidMount() {
        this.setState({ currentDate: DateTime.local().toISODate() });
        this.setState({ newTask: { duration: 0, durationInvalid: false, startTime: "00:00", endTime: "00:00" } });

        window.addEventListener('resize', this.handleWindowSizeChange);
        this.handleWindowSizeChange();

        // Retrieve a list of tasks
        this.retrieveTasks();

        // Retrieve the latest timesheet for the current day
        this.retrieveTimesheets();
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.handleWindowSizeChange);
    }

    handleWindowSizeChange = () => {
        // Modify state which needs to change on window resize event
        const _windowState = window.innerWidth < 500 ? "mobile" : window.innerWidth < 1000 ? "tablet" : "desktop";

        this.setState({
            windowState: _windowState,
            interval: _windowState === "mobile" ? 1 : 7
        });
    };


    componentDidUpdate = (prevProps, prevState) => {
        // Update timesheet when date changes.

        if (this.state.currentDate !== prevState.currentDate) {
            this.retrieveTasks();
            this.retrieveTimesheets();
        }

        if (this.state.newTask !== prevState.newTask) {
            this.calcTimeDiff(this.state.newTask);
        }
    }

    moveDate = (interval) => {
        if (this.state.currentDate !== DateTime.local.toISODate) {
            this.setState({ currentDate: DateTime.fromISO(this.state.currentDate).plus({ days: interval }).toISODate() });
        }
    }

    calcTimeDiff = (newTask) => {
        if (newTask !== undefined) {
            const startTime = DateTime.fromFormat(newTask.startTime, "HH:mm");
            const endTime = DateTime.fromFormat(newTask.endTime, "HH:mm");
            newTask.startTimeInvalid = startTime > endTime;
            newTask.endTimeInvalid = endTime < startTime;

            if (!(newTask.startTimeInvalid || newTask.endTimeInvalid)) {
                newTask.duration = Interval.fromDateTimes(startTime, endTime).length('hours');
                // Round to nearest 0.25
                // Ref: https://stackoverflow.com/questions/1553704/round-to-nearest-25-javascript
                newTask.duration = (Math.round(newTask.duration * 4) / 4).toFixed(2);
                newTask.durationInvalid = !(newTask.duration > 0 && newTask.duration <= 18);
            }

            // Flag set if any one of the validity conditions is satisfied
            newTask.invalidSub = newTask.startTimeInvalid || newTask.endTimeInvalid || newTask.durationInvalid;

            return newTask;
        }
    }


    inputChange = async (event) => {

        let newTask = this.state.newTask;
        switch (event?.target?.id) {
            case "selectedDate":
                await this.setState({ currentDate: event.target.value });
                break;
            case "selectTask":
                newTask.taskID = event.target.value;
                this.setState({ newTask });
                break;
            case "taskDuration":
                newTask.duration = event.target.value;
                newTask.durationInvalid = !(event.target.value >= 0 && event.target.value <= 18);
                this.setState({ newTask });
                break;
            case "startTime":
                newTask.startTime = event.target.value;
                newTask = this.calcTimeDiff(newTask);
                await this.setState({ newTask });
                break;
            case "endTime":
                newTask.endTime = event.target.value;
                newTask = this.calcTimeDiff(newTask);
                await this.setState({ newTask });
                break;
            default:
                console.log(event.target?.id);

                break;
        }
    }

    addTimesheetEntry = async () => {
        this.calcTimeDiff();
        if (this.props?.auth !== undefined && !this.state.newTask.invalidSub) {
            const response = await ax(process.env.REACT_APP_APIURL_TIMESHEETS_ADD, {
                method: 'put',
                data: {
                    assignmentID: Number(this.state.newTask.taskID),
                    date: this.state.currentDate,
                    timeAllocation: Number(this.state.newTask.duration)
                    // startTime: this.state.newTask.startTime,
                    // endTime: this.state.newTask.endTime
                }
            }, this.props.auth.tokenID);

            console.log(response);

            if (response.status === 200) {
                // Update timesheet
                await this.retrieveTimesheets();
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
                if (response.errResponse.status === 400) {
                    await this.setState({ status: ["Invalid data sent through to the API. Please check inputs and try again."] });
                };
            }

        }
    }

    retrieveTasks = async () => {
        if (this.props?.auth !== undefined) {
            const newTask = this.state.newTask;
            const response = await ax(process.env.REACT_APP_APIURL_TASKS_GET, {
                params: {
                    startDate: DateTime.fromISO(this.state.currentDate).minus({ days: 7 }).toISODate()
                }
            }, this.props.auth.tokenID);
            
            if (response?.data !== undefined) {
                await this.setState({ taskData: response.data });

                // Update the Task Input selection
                if (this.state.taskData.length > 0) {
                    newTask.taskID = this.state.taskData[0].taskID;
                }
                await this.setState({ newTask });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    retrieveTimesheets = async () => {
        if (this.props?.auth !== undefined) {
            const response = await ax(process.env.REACT_APP_APIURL_TIMESHEETS_GET, {
                params: {
                    startDate: this.state.currentDate,
                    endDate: this.state.currentDate
                }
            }, this.props.auth.tokenID);
            if (response?.data !== undefined) {
                await this.setState({ timesheetData: response.data });
                await this.setState({ status: [] });
            } else {
                await this.setState({ status: response?.errResponse?.errors || response?.errResponse?.message });
            }
        }
    }

    deleteTimesheetRecord = async () => {
        if (this.props?.auth !== undefined) {
            const response = await ax(process.env.REACT_APP_APIURL_TIMESHEETS_DELETE, {
                method: 'delete',
                params: {
                    id: this.state.timesheetID
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

    resetTimesheetEntry = async () => {
        await this.setState({ newTask: { duration: 0, durationInvalid: true, invalidSub: true, startTime: "00:00", endTime: "00:00" } });
        await this.retrieveTasks();
    }

    tableFunctions = async (callerID, row) => {
        const newTask = this.state.newTask;
        switch (callerID) {
            case ("rowEdit"):
                newTask.taskID = row.assignmentID;
                await this.setState({ newTask });
                break;
            case ("rowDelete"):
                await this.setState({ timesheetID: row.recordID });
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
        }

        return (
            <main>
            <header>
                <AppNavbar mobile={this.state.windowState === "mobile"} />
            </header>
            <article id="">
                <h2 className="titleDate">
                    Timesheet for {DateTime.fromISO(this.state.currentDate).toFormat("cccc, LLL dd yyyy")}
                </h2>

                {/* 
                    Use of onClick events with parameters
                    Ref: https://upmostly.com/tutorials/pass-a-parameter-through-onclick-in-react 
                
                */}
                <FormGroup id="datePicker">
                    <InputGroup>
                        <InputGroupAddon addonType="prepend">
                            <Button id="reverseDate" onClick={() => this.moveDate(-this.state.interval)}><i className="fas fa-angle-left" /></Button>
                        </InputGroupAddon>
                            <Input type="date" name="date" id="selectedDate" max={DateTime.local().toISODate()} value={this.state.currentDate} onChange={(event) => this.inputChange(event)} onClick={(event) => this.inputChange(event)} />
                        <InputGroupAddon addonType="append">
                            <Button id="forwardDate" disabled={this.state.currentDate === DateTime.local().toISODate()} onClick={() => this.moveDate(this.state.interval)}><i className="fas fa-angle-right" /></Button>
                        </InputGroupAddon>
                    </InputGroup>
                </FormGroup>

                <section id="inputTimesheetTime">
                    <h3>Add a time entry</h3>
                    <FormGroup>
                        <Label for="selectTask">Task/Activity</Label>
                        <Input type="select" name="selectTask" id="selectTask" defaultValue={this.state.newTask.taskID} value={this.state.newTask.taskID} onChange={(event) => this.inputChange(event)}>
                            {/* Retrieve the list of tasks for the user's cohort, sorted reverse by endDate */}
                            {this.state.taskData?.map((task, index) => (<option key={index} value={task.taskID}>{task.title}</option>))}
                        </Input>

                        <Label for="startTime">Start Time</Label>
                        <Input type="time" name="startTime" id="startTime" value={this.state.newTask.startTime} onChange={(event) => this.inputChange(event)} />

                        <Label for="endTime">End Time</Label>
                        <Input type="time" name="endTime" id="endTime" value={this.state?.newTask?.endTime} onChange={(event) => this.inputChange(event)} />

                        <FormGroup>
                            <Label for="taskDuration">Duration (to the nearest 15min)</Label>
                            <Input plaintext name="taskDuration" id="taskDuration" value={Duration.fromObject({ hours: this.state.newTask.duration }).toFormat("hh:mm")} disabled invalid={this.state.newTask.invalidSub} />
                            <FormFeedback>Task duration cannot be 0 or above 18, please correct start and end times.</FormFeedback>
                        </FormGroup>

                        <Alert color="danger" hidden={!(this.state.status?.length > 0)}>
                            <ul>
                                {Array.isArray(this.state?.status) ? this.state.status.map((status, index) => (<li key={index}>{status}</li>)) : null}
                            </ul>
                        </Alert>

                        <Button id="addTimesheetEntry" color="info" onClick={() => this.addTimesheetEntry()} disabled={this.state.newTask.invalidSub}><i className="fas fa-plus" /></Button>
                        <Button id="clearTimesheetEntry" onClick={() => this.resetTimesheetEntry()}>Clear</Button>

                    </FormGroup>
                </section>




                <section id="tableTimesheetOverview">
                    <h3>My activities</h3>
                    <TimesheetTable timesheetData={this.state.timesheetData} mobile={this.state.windowState === "mobile"} taskData={this.state.taskData} func={this.tableFunctions} />
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
    )(Timesheets));

