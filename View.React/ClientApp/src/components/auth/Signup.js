import React, { Component } from "react";
import { NavLink, Navigate } from "react-router-dom";
import {
    Button,
    Form,
    FormGroup,
    Label,
    Input,
    Container,
    Row,
    Col,
    Alert,
} from "reactstrap";
import AuthContext from "./AuthContext";
import { api } from "../misc/Api";
import { handleLogError } from "../misc/Helpers";
import jwtDecode from "jwt-decode";

class Signup extends Component {
    static contextType = AuthContext;

    state = {
        name: "",
        email: "",
        password: "",
        confirmPassword: "",
        isLoggedIn: false,
        isError: false,
        errorMessage: "",
    };

    componentDidMount() {
        const Auth = this.context;
        const isLoggedIn = Auth.userIsAuthenticated();
        this.setState({ isLoggedIn });
    }

    handleInputChange = (e) => {
        const { name, value } = e.target;
        this.setState({ [name]: value });
    };

    handleSubmit = (e) => {
        e.preventDefault();

        const { name, email, password, confirmPassword } = this.state;

        if (!(email && password && confirmPassword)) {
            this.setState({
                isError: true,
                errorMessage: "Please, fill in all fields!",
            });
            return;
        }

        if (password !== confirmPassword) {
            this.setState({
                isError: true,
                errorMessage: "Passwords do not match!",
            });
            return;
        }

        api.register(name, email, password)
            .then((response) => {
                const token = response.data.token;
                const decodedToken = jwtDecode(token);
                const name = decodedToken["Name"];
                const email =
                    decodedToken[
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
                    ];
                const role =
                    decodedToken[
                        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    ];
                const user = { name, email, role, token };

                const Auth = this.context;
                Auth.userLogin(user);

                this.setState({
                    name: "",
                    email: "",
                    password: "",
                    confirmPassword: "",
                    isLoggedIn: true,
                    isError: false,
                    errorMessage: "",
                });
            })
            .catch((error) => {
                handleLogError(error);
                if (error.response && error.response.data) {
                    const errorData = error.response.data;
                    let errorMessage = "Invalid fields";
                    if (error.response.status === 401) {
                        errorMessage = errorData.message;
                    } else if (error.response.status === 400) {
                        errorMessage = errorData.errors[0].defaultMessage;
                    }
                    this.setState({
                        isError: true,
                        errorMessage,
                    });
                }
            });
    };

    render() {
        const { isLoggedIn, isError, errorMessage } = this.state;
        if (isLoggedIn) {
            return <Navigate to='/dashboard' />;
        } else {
            return (
                <Container className='signup'>
                    <h1 className='text-center'>Register an account</h1>
                    <Row className='justify-content-center'>
                        <Col xs='12' sm='8' md='6' lg='4'>
                            <Form onSubmit={this.handleSubmit}>
                                <FormGroup>
                                    <Label for='name'>Name</Label>
                                    <Input
                                        type='text'
                                        name='name'
                                        id='name'
                                        placeholder='Name'
                                        value={this.state.name}
                                        onChange={this.handleInputChange}
                                    />
                                </FormGroup>
                                <FormGroup>
                                    <Label for='email'>E-Mail</Label>
                                    <Input
                                        type='text'
                                        name='email'
                                        id='email'
                                        placeholder='E-Mail'
                                        value={this.state.email}
                                        onChange={this.handleInputChange}
                                    />
                                </FormGroup>
                                <FormGroup>
                                    <Label for='password'>Password</Label>
                                    <Input
                                        type='password'
                                        name='password'
                                        id='password'
                                        placeholder='Password'
                                        value={this.state.password}
                                        onChange={this.handleInputChange}
                                    />
                                </FormGroup>
                                <FormGroup>
                                    <Label for='confirmPassword'>
                                        Confirm Password
                                    </Label>
                                    <Input
                                        type='password'
                                        name='confirmPassword'
                                        id='confirmPassword'
                                        placeholder='Confirm Password'
                                        value={this.state.confirmPassword}
                                        onChange={this.handleInputChange}
                                    />
                                </FormGroup>
                                <Button color='primary' block>
                                    Signup
                                </Button>
                            </Form>
                            <p className='mt-3'>
                                Already have an account?{" "}
                                <NavLink to='/login'>Login</NavLink>
                            </p>
                            {isError && (
                                <Alert color='danger'>{errorMessage}</Alert>
                            )}
                        </Col>
                    </Row>
                </Container>
            );
        }
    }
}

export default Signup;
