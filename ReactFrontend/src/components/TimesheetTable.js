import React from 'react';
import { Duration } from 'luxon';

import { Table, Alert, Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback } from 'reactstrap';


class TimesheetTable extends React.Component {

    render() {
        if (this.props.timesheetData?.length === 0) {
            // If there is no data to display
            return (
                <Alert color="light">Nothing to display.</Alert>
            )
        } else {
            // Display the results table
            return (
                <Table id="timesheetData" size={this.props.size === "mobile" ? "sm" : null} bordered>
                    <thead>
                        <tr>
                            <th>#</th>
                            <th hidden={this.props.mobile}>Start time</th>
                            <th hidden={this.props.mobile}>End time</th>
                            <th>Activity</th>
                            <th>Duration (h)</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.props.timesheetData?.map((row, index) => {
                            return (
                                <tr key={row.recordID}>
                                    <td>{index}</td>
                                    <td hidden={this.props.mobile}>{row?.startTime}</td>
                                    <td hidden={this.props.mobile}>{row?.endTime}</td>
                                    <td key={row?.taskID}>{
                                        this.props?.taskData?.map(taskObject => {
                                            return (taskObject.taskID === row.assignmentID) ? taskObject.title : null
                                        }).filter(taskObject => taskObject !== null)
                                    }</td>
                                    <td>{Duration.fromObject({ hours: row.timeAllocation }).toFormat("hh:mm")}</td>
                                    <td>
                                        <FormGroup>
                                            <Button id="rowEdit" name="rowEdit" className="tableControls" onClick={() => this.props.func("rowEdit", row)}><i className="fas fa-edit"></i></Button>
                                            <Button id="rowDelete" name="rowDelete" className="tableControls" onClick={() => this.props.func("rowDelete", row)}><i className="fas fa-trash"></i></Button>
                                        </FormGroup>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </Table>
            );
        }
    }

}

export default TimesheetTable;