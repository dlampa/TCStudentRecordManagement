import React from 'react';
import { connect } from 'react-redux';
import { withRouter, Link, Redirect } from 'react-router-dom';
import GoogleLogin from 'react-google-login';
import axios from 'axios';

// Actions
import { loginUser } from './actions';

import './App.css';



class App extends React.Component {
  constructor(props) {
    super(props);


    this.state = {
      currentUser: null
    }
  }
  
  render() {

    return (
      <main className="App">
        <h1>Welcome to TECHCareers</h1>
        <h2>Student Record Management System</h2>
        <GoogleLogin clientId="553228721119-dp1p9m24d2br2it12un57pep3gomtgp1.apps.googleusercontent.com"
          buttonText="Login using your UofA account"
          onSuccess={this.registerUserInState}
          onFailure={this.responseGoogle}
          isSignedIn={true}
          uxMode={'redirect'}
          cookiePolicy={'single_host_origin'} />


        {(this.state.currentUser != null ? console.log(this.state.currentUser) : <div>No User data loaded</div>)}

      </main>
    );
  }
  

  registerUserInState = (response) => {
    
    // Take the authentication token received from Google and pass it to API target /auth
    axios.get("https://localhost:5001/auth/logon", { headers: { 'Authorization': `Bearer ${response.tokenId}` } })
      .then(response => {
        console.log(response);
        if (response.status === 200) {
          console.log("works");
        } else {
          console.log("doesn't");
        }
      })
      .catch(err => { console.log('error'); console.log(err.response) });
      
      this.props.dispatch(loginUser(response));
      //this.props.history.push(process.env.PUBLIC_URL + "/timesheets/");
    
      // Check response. If valid, receive user credential level and compare to what was received from Google
    
    // Take the authentication token received from Google and store in Redux store

    // If API authentication has failed, return the user to the login screen.
    
    
    

  }

  
  responseGoogle = (response) => {   
    this.setState({ currentUser: response });
    this.getApiData();
  }
  
  getApiData = () => {
    //   const requestConfig = {
    //     method: 'put',
    //     url: 'https://localhost:5001/cohorts/list',
    //     headers: {
    //        'Authorization': `Bearer ${token.tokenId}`
    //     }
    //   };
    
    //   axios(requestConfig)
    //     .then(apiresponse => console.log(apiresponse))
    //     .catch(err => console.log(err));
    // }
  }
}



export default withRouter(connect((state) => { return { state: state }})(App));
