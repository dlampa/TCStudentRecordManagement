import React from 'react';

import '../css/StudentsTable.css';

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

                <ul id="studentData">
                    {this.props.studentData?.map((row, index) => {
                        return (
                            <li key={row.studentID} className="listItem">
                                <div className="listRecord">{index + 1}</div>
                                <div className="listData">
                                    <div className="listName">{row.user.firstname} {row.user.lastname}</div>
                                    <div className="listEmail"><a href={"mailto:"+ row.user.email}>{row.user.email}</a></div>
                                    <div className="listBeartracks">{row.bearTracksID}</div>
                                    <div className="listFooter">
                                        <div className="activeStatus">{row.user.active ? "Active" : "Inactive"}</div>
                                        <div className="listControls">
                                            <button className="studentEdit" onClick={() => this.props.func("studentEdit", row)}><i className="fas fa-edit"></i></button>
                                            <button className="studentState" onClick={() => this.props.func("studentState", row)}><i className="fas fa-user-alt-slash"></i></button>
                                            <button className="studentDelete" hidden={this.props.role !== "SuperAdmin"} onClick={() => this.props.func("studentDelete", row)}><i className="fas fa-user-times"></i></button>
                                        </div>
                                    </div>
                                </div>
                            </li>

                        )
                    })}
                </ul>
            );
        }
    }
}

export default StudentsTable;