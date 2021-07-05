import React from 'react';
import { connect } from 'react-redux';
import GoogleLogin from 'react-google-login';
import axios from 'axios';

// Actions
import { loginUser } from '../actions';

class UserLogin extends React.Component 
{
    constructor(props)  {
        super(props);

        // this.state={};

    }

    render() {
        return(
          <GoogleLogin clientId="553228721119-dp1p9m24d2br2it12un57pep3gomtgp1.apps.googleusercontent.com"
          buttonText="Login using your UofA account"
          onSuccess={this.authUserWithAPI}
          onFailure={this.authFailedHandler}
          isSignedIn={true}
          uxMode={'redirect'} // redirect
          cookiePolicy={'single_host_origin'} />
        );
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
    
          }
        }
        catch (err) {
          // If API authentication has failed, display the error message to the user
          //const errList = this.state.errors;
    
          //this.props.dispatch(loginUser(null));
          //errList.push(`${err?.response?.data?.status ?? ""}Unauthorized user login. If you believe this has happened in error, please contact our staff.`);
          //this.setState({ errors: errList });
    
          //this.setState({ loggedIn: false }, () => true);
        }
      }

    authFailedHandler = (response) => {
        const errList = this.state.errors;
        errList.push(response);
        this.setState({ errors: errList });
        }
}

export default UserLogin;