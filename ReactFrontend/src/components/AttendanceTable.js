import React from 'react';

import AttendanceStatusList from './AttendanceStatusList';

import '../css/AttendanceTable.css';

import { Table, Alert, Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback } from 'reactstrap';


class AttendanceTable extends React.Component {

    render() {
        if (this.props.studentData?.length === 0) {
            // If there is no data to display
            return (
                <Alert color="light">Nothing to display.</Alert>
            )
        } else {
            // Display the results table
            return (

                <ul id="attendanceData">
                    {this.props.studentData?.map((row, index) => {
                        return (
                            <li className="attListItem" key={row.studentID}>
                                <div className="attListRecord">{index + 1}</div>
                                <div className="attListData">
                                    <div className="attListName">
                                        {row.user.firstname} {row.user.lastname}
                                    </div>
                                    <div className="attListAttStatus">
                                        <AttendanceStatusList data={this.props.attendanceStatesData} func={() => this.props.func("selectStatus")} />
                                    </div>
                                    <div className="attListComments">
                                        <button className="attAddEditComment" onClick={() => this.props.func("commentEdit", row)}><i className="fas fa-edit"></i></button>
                                    </div>
                                </div>
                                <div className="attListAttAck">
                                        <Input type="checkbox" name={"ackAttendance_" + row.studentID} title="Acknowledge attendance" onClick={() => this.props.func("updateStatus")} />
                                </div>

                            </li>
                        )
                    })}
                </ul>
            );
        }
    }
}

export default AttendanceTable;