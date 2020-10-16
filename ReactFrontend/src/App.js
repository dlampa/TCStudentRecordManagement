import React from 'react';
import './App.css';
import GoogleLogin from 'react-google-login';
import axios from 'axios';

var token;


class App extends React.Component {
  constructor(props) {
    super(props);


    this.state = {
      currentUser: ""
    }
  }
  
  render() {
    return (
      <main className="App">
        <h1>Welcome to TECHCareers</h1>
        <h2>Student Record Management System</h2>
        <GoogleLogin clientId="553228721119-dp1p9m24d2br2it12un57pep3gomtgp1.apps.googleusercontent.com"
          buttonText="Login using your UofA account"
          onSuccess={responseGoogle}
          onFailure={responseGoogle}
          cookiePolicy={'single_host_origin'} />

      </main>
    );
  }
}



function responseGoogle(response) {
  console.log(response);
  token = response;
  getApiData();
}

function getApiData() 
{
  const requestConfig = {
    method: 'put',
    url: 'https://localhost:5001/cohorts/add',
    params: {name: "test", startDate: "2020-07-02", endDate: "2020-08-03"},
    headers: {
       'Authorization': `Bearer ${token.tokenId}`
    }
  };
  
  axios(requestConfig)
    .then(apiresponse => console.log(apiresponse))
    .catch(err => console.log(err));
}


export default App;
