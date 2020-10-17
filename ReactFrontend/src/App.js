import React from 'react';
import { connect } from 'react-redux';
import { withRouter, Link, Redirect } from 'react-router-dom';
import { Alert } from 'reactstrap';
import GoogleLogin from 'react-google-login';
import axios from 'axios';

// Actions
import { loginUser } from './actions';

import './App.css';



class App extends React.Component {
  constructor(props) {
    super(props);


    this.state = {
      errors: []
    };
  }

  render() {

    return (
      <main className="App">
        <h1>Welcome to TECHCareers</h1>
        <h2>Student Record Management System</h2>
        <div>
        <GoogleLogin clientId="553228721119-dp1p9m24d2br2it12un57pep3gomtgp1.apps.googleusercontent.com"
          buttonText="Login using your UofA account"
          onSuccess={this.authUserWithAPI}
          onFailure={this.authFailedHandler}
          isSignedIn={true}
          uxMode={'redirect'}
          cookiePolicy={'single_host_origin'} />
        </div>
        
        {this.state.errors.map((error, index) => <Alert color="danger" key={index}>{error}</Alert>)}


      </main>
    );
  }


  authUserWithAPI = async (response) => {
    // Take the authentication token received from Google and pass it to API target /auth
    try {
      const apiResponse = await axios.get("https://localhost:5001/auth/logon",
        { headers: { 'Authorization': `Bearer ${response.tokenId}` } });

      // Check response. If valid, receive user credential level and compare to what was received from Google
      if (apiResponse.status === 200 & response.tokenId === apiResponse.data.tokenID) {

        // Attach additional information received from google to the API response
        apiResponse.data.tokenID = response.tokenId;
        apiResponse.data.tokenID = response.tokenId;

        // Take the authentication token received from Google and store in Redux store
        this.props.dispatch(loginUser(apiResponse));

        // Redirect user to the home page
        this.props.history.push(process.env.PUBLIC_URL + "/timesheets/");
      }
    }
    catch (err) {
      // If API authentication has failed, display the error message to the user
      //console.log(err.response.data.status);
      console.log(err.response);
      const errList = this.state.errors;
      //this.props.dispatch(loginUser(null));
      errList.push(`${err?.response?.data?.status??""}Unauthorized user login. If you believe this has happened in error, please contact our staff.`);

      this.setState({ errors: errList });
    }


    //  .catch(err => { console.log('error'); console.log(err.response) });
    //this.props.dispatch(loginUser(response));




  }

  authFailedHandler = (response) => {
    console.log("Failed" + response);
  }

}



export default withRouter(connect((state) => { return { state: state } })(App));
