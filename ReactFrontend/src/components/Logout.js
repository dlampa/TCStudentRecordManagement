import React from 'react';
import { withRouter, Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import { logoffUser } from '../actions/';
import { GoogleLogout } from 'react-google-login';


class Logout extends React.Component {

    componentDidMount = () => {
        this.props.dispatch(logoffUser());
        
        // Remove all cookies from the session
        // Ref: https://www.jsdiaries.com/how-to-remove-all-cookies-in-react-js/
        document.cookie.split(";").forEach((c) => {
            document.cookie = c
                .replace(/^ +/, "")
                .replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
        });
    }

    redirectToLogin = () => {
        this.props.history.push("/");
    }
    render() {
        return (
            <>
                <div>Thank you for using myTECHCareers.</div>
                <GoogleLogout autoload={true} onLogoutSuccess={this.redirectToLogin}/>
            </>
        )
    }
}


export default withRouter(connect((state) => { return { auth: state.auth } })(Logout));