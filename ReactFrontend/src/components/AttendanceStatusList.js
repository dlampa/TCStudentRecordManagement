import React from 'react';
import { Input } from 'reactstrap';

import '../css/AttendanceStatusList.css';

class AttendanceStatusList extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            attendanceStateID: 0
        };
    }

    //componentDidMount = () => this.setState({ attendanceStateID: this.props?.data[0]?.stateID });

    updateState = (event) => {
        this.setState({ attendanceStateID: event.target.value })
    }

    render() {
        return (
            <Input type="select" className="selectAttendanceState" onChange={(event) => this.updateState(event)} value={this.state.attendanceStateID} defaultValue={this.state.attendanceStateID}>
                {this.props.data?.map((attendanceState, index) => {
                    return <option key={index} value={attendanceState.stateID}>{attendanceState.description}</option>
                }
                )};
            </Input>
        );
    }

}

export default AttendanceStatusList;