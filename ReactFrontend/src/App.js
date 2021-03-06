import React from 'react';
import { connect } from 'react-redux';
import { withRouter, Link, Redirect } from 'react-router-dom';
import { Alert } from 'reactstrap';
import GoogleLogin from 'react-google-login';
import axios from 'axios';
import UserLogin from './components/UserLogin';

// Actions
import { loginUser } from './actions';

import './css/App.css';


class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      loggedIn: false,
      errors: []
    };
  }

  componentDidMount = () => {
      console.log("componentDidMount");
      console.log("---");
      if (this.props.location.hash.length > 0 ) {
        var tokenId = this.props.location.hash.match('id_token=(.+)&')[1];
        console.log(tokenId);
        this.authUserWithAPI2(tokenId);


      }
  }

  render() {
    console.log(this.state);
    
    console.log(this.props.location);

    if (this.state.loggedIn === undefined || this.state.loggedIn === false) {
      return (
        <main className="App">
          <div>
            <h1>Welcome to myTECHCareers</h1>
            {/* <UserLogin /> */}
            <GoogleLogin clientId="553228721119-dp1p9m24d2br2it12un57pep3gomtgp1.apps.googleusercontent.com"
              buttonText="Login using your UofA account"
              onSuccess={this.authUserWithAPI}
              onFailure={this.authFailedHandler}
              isSignedIn={true}
              uxMode={'redirect'} // redirect or popup
              redirectUri="http://localhost:3000/auth/"
              cookiePolicy={'single_host_origin'} />
          </div>
        </main>
      );
    } else {
      var destination=this.props.auth.rights === "Student" ? "/timesheets" : "/students";
      return (
        <>
          <p>Please wait...</p>
          {// this.props.auth.rights == "Student" ? this.props.history.push(process.env.PUBLIC_URL + "/timesheets/") : this.props.history.push(process.env.PUBLIC_URL + "/students/") 
          }
          <Redirect push to={{ pathname: destination }} />
        </>
      );
    }
  }

  authUserWithAPI = async (response) => {
    // Take the authentication token received from Google and pass it to API target /auth
    try {
      const apiResponse = await axios.get("https://localhost:5001/auth/logon",
        { headers: { 'Authorization': `Bearer ${response.tokenId}` } });
      
        console.log("Here");

      // Check response. If valid, receive user credential level and compare to what was received from Google
      if (apiResponse.status === 200) {

        // Take the authentication token received from API and store in Redux store
        await this.props.dispatch(loginUser(apiResponse));
        await this.setState({ loggedIn: true }); //, () => this.props.history.push(process.env.PUBLIC_URL));
        //this.props.history.push(process.env.PUBLIC_URL);

      }
    }
    catch (err) {
      // If API authentication has failed, display the error message to the user
      const errList = this.state.errors;

      //this.props.dispatch(loginUser(null));
      errList.push(`${err?.response?.data?.status ?? ""}Unauthorized user login. If you believe this has happened in error, please contact our staff.`);
      this.setState({ errors: errList });

      this.setState({ loggedIn: false }, () => true);
    }

  }

  authUserWithAPI2 = (tokenId) => {
    // Take the authentication token received from Google and pass it to API target /auth
    try {
      const apiResponse = axios.get("https://localhost:5001/auth/logon",
        { headers: { 'Authorization': `Bearer ${tokenId}` } });
      
        console.log("Here");

      // Check response. If valid, receive user credential level and compare to what was received from Google
      if (apiResponse.status === 200) {

        // Take the authentication token received from API and store in Redux store
        this.props.dispatch(loginUser(apiResponse));
        this.setState({ loggedIn: true }, ()=> true); //, () => this.props.history.push(process.env.PUBLIC_URL));
        //this.props.history.push(process.env.PUBLIC_URL);

      }
    }
    catch (err) {
      // If API authentication has failed, display the error message to the user
      const errList = this.state.errors;

      //this.props.dispatch(loginUser(null));
      errList.push(`${err?.response?.data?.status ?? ""}Unauthorized user login. If you believe this has happened in error, please contact our staff.`);
      this.setState({ errors: errList });

      this.setState({ loggedIn: false }, () => true);
    }

  }



  authFailedHandler = (response) => {
    const errList = this.state.errors;
    errList.push(response);
    this.setState({ errors: errList })
  }

}



export default withRouter(
  connect(
    (state) => {
      return {
        loggedIn: (state.auth?.tokenID !== undefined && state.auth.tokenID !== null),
        auth: state.auth
      }
    }
  )(App));
