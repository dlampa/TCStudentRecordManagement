import React from 'react';
import ReactDOM from 'react-dom';

// CSS
import './css/reset.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import './css/Typography.css';

// Redux setup
import { createStore } from 'redux';
import { Provider } from 'react-redux';
import TCDataReducer from './reducer';

// React-Router setup
import { BrowserRouter as Router, Route, Switch, Redirect } from 'react-router-dom';

// App components
import App from './App';
import Timesheets from './components/Timesheets';
import Logout from './components/Logout';
import Students from './components/Students';


const store = createStore(TCDataReducer, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()); // Redux devtools

const Root = (store) => (
  <Provider store={store.store}>
    <Router>
      <Switch>
      <Route path={process.env.PUBLIC_URL + "/"} exact component={App} />
        <Route path={process.env.PUBLIC_URL + "/timesheets/"} exact component={Timesheets} />
        <Route path={process.env.PUBLIC_URL + "/students/"} exact component={Students} />
        {/*
        <Route path={process.env.PUBLIC_URL + "/timeline/"} component={Timesheets} />
        <Route path={process.env.PUBLIC_URL + "/search/:searchTerm"} component={SearchPage} /> */}
      <Route path={process.env.PUBLIC_URL + "/logout/"} exact component={Logout} />
      <Route component={() => (<Redirect to="/"/>) } />
      </Switch>
    </Router>
  </Provider>
);


ReactDOM.render(
  <Root store={store} />,
  document.getElementById('root')
);
