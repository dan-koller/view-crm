import React, { Component } from "react";
import { NavLink, Navigate } from "react-router-dom";
import {
    Col,
    Container,
    Row,
    Form,
    FormGroup,
    Label,
    Input,
    Button,
    Alert,
} from "reactstrap";
import AuthContext from "./AuthContext";
import { handleLogError } from "../misc/Helpers";
import jwtDecode from "jwt-decode";
import { api } from "../misc/Api";

class Login extends Component {
    static contextType = AuthContext;

    state = {
        username: "",
        password: "",
        isLoggedIn: false,
        isError: false,
    };

    componentDidMount() {
        const Auth = this.context;
        const isLoggedIn = Auth.userIsAuthenticated();
        this.setState({ isLoggedIn });
    }

    handleInputChange = (event) => {
        const { name, value } = event.target;
        this.setState({ [name]: value });
    };

    handleSubmit = (e) => {
        e.preventDefault();

        const { username, password } = this.state;
        if (!(username && password)) {
            this.setState({ isError: true });
            return;
        }

        api.authenticate(username, password)
            .then((response) => {
                const token = response.data.token;
                const decodedToken = jwtDecode(token);
                // Fullname
                const name = decodedToken["Name"];
                // Username
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
                    username: "",
                    password: "",
                    isLoggedIn: true,
                    isError: false,
                });
            })
            .catch((error) => {
                handleLogError(error);
                this.setState({ isError: true });
            });
    };

    render() {
        const { isLoggedIn, isError } = this.state;
        if (isLoggedIn) {
            return <Navigate to='/dashboard' />;
        } else {
            return (
                <Container className='login'>
                    <h1 className='text-center'>Login with your account</h1>
                    <Row className='justify-content-center'>
                        <Col xs={12} md={6} lg={4}>
                            <Form onSubmit={this.handleSubmit}>
                                <FormGroup>
                                    {/* TODO: Change to email for better consistency */}
                                    <Label for='username'>Username</Label>
                                    <Input
                                        type='text'
                                        name='username'
                                        id='username'
                                        placeholder='Username'
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
                                        onChange={this.handleInputChange}
                                    />
                                </FormGroup>
                                <Button color='primary' block>
                                    Login
                                </Button>
                            </Form>
                            <p className='mt-3'>
                                Don't have an account?{" "}
                                <NavLink to='/register'>Sign Up</NavLink>
                            </p>
                            {isError && (
                                <Alert color='danger'>
                                    The username or password provided are
                                    incorrect!
                                </Alert>
                            )}
                        </Col>
                    </Row>
                </Container>
            );
        }
    }
}

export default Login;
