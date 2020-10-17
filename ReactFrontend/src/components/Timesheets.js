import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { DateTime } from 'luxon';
import { Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback } from 'reactstrap';

import { get } from '../js';




class Timesheets extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            currentDate: DateTime.local(),
            timesheetModifiedButNotSaved: false,
            timesheetData: null
        };
    }

    // Listen for resize events, then update the isMobile state depending on screen width
    // Ref: https://goshakkk.name/different-mobile-desktop-tablet-layouts-react/
    componentDidMount() {
        this.setState({ currentDate: DateTime.local() });

        window.addEventListener('resize', this.handleWindowSizeChange);
        this.handleWindowSizeChange();

        // Retrieve a list of tasks
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
            interval: _windowState === "mobile" ? 1 : 7
        });
    };

    moveDate = (interval) => this.setState({ currentDate: this.state.currentDate.plus({ days: interval }) });

    inputChange = (event) => {
        switch (event.target.id) {
            case "taskDuration":
                this.setState({
                    newTask: { duration: event.target.value, durationValid: !(event.target.value >= 0 && event.target.value <= 18) }
                });
                break;
            default:
                console.log(event);
                break;
        }
    }

    retrieveTasks = async () => {
        if (this.props?.auth !== undefined) {

            const response = await get(process.env.REACT_APP_APIURL_TASKS_GET, {
                params: {
                    startDate: this.state.currentDate.minus({ days: 7 }).toISODate()
                }
            }, this.props.auth.tokenID);

            if (response?.data !== undefined) {
                await this.setState({ tasks: response.data });
            } else {
                console.log("error?");
            }
        }
    }

    retrieveTimesheets = async () => {
        if (this.props?.auth !== undefined) {
            const response = await get(process.env.REACT_APP_APIURL_TIMESHEETS_GET, {
                params: {
                    startDate: this.state.currentDate.minus({ days: 7 }).toISODate()
                }
            }, this.props.auth.tokenID);
            console.log(response);
            await this.setState({ tasks: response.data });
        }
    }

    render() {

        // Redirect to first page if login data is missing
        if (this.props?.auth?.tokenID === undefined) {
            this.props.history.push(process.env.PUBLIC_URL);
        }

        return (
            <article>
                <h2 className="titleDate">
                    Timesheet for {this.state.currentDate.toLocaleString(DateTime.DATE_MED)}
                </h2>

                {/* 
                    Use of onClick events with parameters
                    Ref: https://upmostly.com/tutorials/pass-a-parameter-through-onclick-in-react 
                
                */}
                <FormGroup>
                    <Label for="selectedDate">Date</Label>
                    <InputGroup>
                        <InputGroupAddon addonType="prepend">
                            <Button id="reverseDate" onClick={() => this.moveDate(-this.state.interval)}><i className="fas fa-angle-left" /></Button>
                        </InputGroupAddon>
                        <Input type="date" name="date" id="selectedDate" defaultValue={this.state.currentDate.toISODate()} />
                        <InputGroupAddon addonType="append">
                            <Button id="forwardDate" onClick={() => this.moveDate(this.state.interval)}><i className="fas fa-angle-right" /></Button>
                        </InputGroupAddon>
                    </InputGroup>
                </FormGroup>

                <section>
                    <h3>Add a time entry</h3>
                    <FormGroup>
                        <Label for="selectTask">Task/Activity</Label>
                        <Input type="select" name="selectTask" id="selectTask" onChange={(task) => console.log(task.target.value)}>
                            {/* Retrieve the list of tasks for the user's cohort, sorted reverse by endDate */}
                            {this.state.tasks?.map((task, index) => (<option key={index} value={task.taskID}>{task.title}</option>))}
                        </Input>

                        <Label for="startTime">Start Time</Label>
                        <Input type="time" name="startTime" id="startTime" />

                        <Label for="endTime">Start Time</Label>
                        <Input type="time" name="endTime" id="endTime" />

                        <FormGroup>
                            <Label for="taskDuration">Duration (hours, to the nearest 0.25)</Label>
                            <Input type="number" name="taskDuration" id="taskDuration" onChange={(event) => this.inputChange(event)} invalid={this.state?.newTask?.durationValid} />
                            <FormFeedback>Entries below 0 and above 18 are not allowed.</FormFeedback>
                        </FormGroup>

                        <Button id="addTimesheetEntry" color="info" onClick={null}><i className="fas fa-plus" /></Button>
                        <Button id="clearTimesheetEntry" onClick={null}>Clear</Button>

                    </FormGroup>
                </section>
            </article>
        );
    }
}



export default withRouter(
    connect(
        (state) => {
            return { auth: state.auth }
        }
    )(Timesheets));

