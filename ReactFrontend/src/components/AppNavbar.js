import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter, Link } from 'react-router-dom';
import { DateTime, Interval, Duration } from 'luxon';
import { Input, Label, Button, Media, Navbar, NavbarBrand, Collapse, Nav, NavItem, NavLink } from 'reactstrap';

import '../css/Navbar.css';

class AppNavbar extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            navbarOpen: false
        };
    }

    toggleNav = () => this.setState({ navbarOpen: !this.state.navbarOpen });

    render() {
        
        // Redirect to first page if login data is missing
        if (!this.props?.auth) {
            this.props.history.push(process.env.PUBLIC_URL);
            return null;
        }
        else {
            return (
                <Navbar dark color="dark" className="d-flex justify-content-between" role="navigation" id="navbarContainer">
                    <NavbarBrand>myTECHCareers</NavbarBrand>
                    <img src={this.props.auth?.imageURL} onClick={this.toggleNav} id="navbarIcon" alt={this.props.auth.fullName} hidden={this.props.auth.imageURL === null} />
                    <i className="fas fa-user" hidden={this.props?.auth?.imageURL !== null} onClick={this.toggleNav} id="navbarIconNoProfilePic" />
                    <Collapse isOpen={this.state.navbarOpen} navbar>
                        <Nav fill={this.props.mobile} justified={true} card={true} navbar>
                            <NavItem hidden={this.props.auth.rights !== "Student"}>
                                <NavLink tag={Link} to="/timesheets/" active={true}>Timesheets</NavLink>
                            </NavItem>
                            {/* Staff items */}
                            {this.props.auth.rights !== "Student" ? (
                                <>
                                    <NavItem>
                                        <NavLink tag={Link} to="/students/">Students</NavLink>
                                    </NavItem>
                                </>
                            ) : null}
                            <NavItem>
                                <NavLink tag={Link} to="/logout/">Log out</NavLink>
                            </NavItem>
                        </Nav>
                    </Collapse>
                </Navbar>
            );
        }
        
    }
}

export default withRouter(
    connect(
        (state) => {
            return { auth: state.auth }
        }
    )(AppNavbar));