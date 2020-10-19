import React from 'react';
import { Duration } from 'luxon';

import { Table, Alert, Input, FormGroup, Label, Button, InputGroup, InputGroupAddon, FormFeedback } from 'reactstrap';


class StudentsTable extends React.Component {

    render() {
        if (this.props.studentData?.length === 0) {
            // If there is no data to display
            return (
                <Alert color="light">Nothing to display.</Alert>
            )
        } else {
            // Display the results table
            return (
                <Table id="studentData" size={this.props.size === "mobile" ? "sm" : null} bordered>
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Surname</th>
                            <th hidden={this.props.mobile}>Email</th>
                            <th hidden={this.props.mobile}>Active</th>
                            <th hidden={this.props.mobile}>BearTracksID</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.props.studentData?.map((row, index) => {
                            return (
                                <tr key={row.studentID}>
                                    <td>{index + 1}</td>
                                    <td>{row.user.firstname}</td>
                                    <td>{row.user.lastname}</td>
                                    <td hidden={this.props.mobile}>{row.user.email}</td>
                                    <td hidden={this.props.mobile}>{row.user.active}</td>
                                    <td hidden={this.props.mobile}>{row.bearTracksID}</td>
                                    <td>
                                        <FormGroup>
                                            <Button id="studentEdit" name="studentEdit" className="tableControls" onClick={() => this.props.func("studentEdit", row)}><i className="fas fa-edit"></i></Button>
                                            <Button id="studentState" name="studentState" className="tableControls" onClick={() => this.props.func("studentState", row)}><i className="fas fa-user-alt-slash"></i></Button>
                                            <Button id="studentDelete" name="studentDelete" className="tableControls" onClick={() => this.props.func("studentDelete", row)}><i className="fas fa-user-times"></i></Button>
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

export default StudentsTable;