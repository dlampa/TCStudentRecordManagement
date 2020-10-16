import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';


// Redux setup
import { createStore } from 'redux';
import { Provider } from 'react-redux';
import TCDataReducer from './reducer';

// React-Router setup
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';

// App components
import App from './App';
import Timesheets from './components/Timesheets';


const store = createStore(TCDataReducer, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()); // Redux devtools

const Root = (store) => (
  <Provider store={store.store}>
    <Router>
      <Route path={process.env.PUBLIC_URL + "/"} exact component={App} />
      <Switch>
        <Route path={process.env.PUBLIC_URL + "/timesheets"} component={Timesheets} />
        {/*
        <Route path={process.env.PUBLIC_URL + "/profile/:userid"} component={UserProfilePage} />
        <Route path={process.env.PUBLIC_URL + "/timeline/"} component={Timesheets} />
        <Route path={process.env.PUBLIC_URL + "/search/:searchTerm"} component={SearchPage} /> */}
      </Switch>
    </Router>
  </Provider>
);


ReactDOM.render(
  <Root store={store} />,
  document.getElementById('root')
);
