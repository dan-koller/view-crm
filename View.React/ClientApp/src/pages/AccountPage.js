import { useState, useEffect, useContext } from "react";
import { redirect, useNavigate, useParams } from "react-router-dom";
import CategoriesContext from "../context";
import { useAuth } from "../components/auth/AuthContext";
import { api } from "../components/misc/Api";
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

const AccountPage = () => {
    const [state, setState] = useState({
        name: "",
        email: "",
        password: "",
        newPassword: "",
        confirmPassword: "",
    });

    const [successMessage, setSuccessMessage] = useState("");
    const [errorMessage, setErrorMessage] = useState("");

    const auth = useAuth();
    const navigate = useNavigate();

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setState({
            ...state,
            [name]: value,
        });
    };

    const handleFormSubmit = (e) => {
        e.preventDefault();

        // Reset messages
        setSuccessMessage("");
        setErrorMessage("");

        let currentUserName = auth.getUser().name;
        let userWantToChangeName = state.name !== currentUserName;
        let userWantToChangePassword = state.newPassword.length > 0;

        if (!userWantToChangeName && !userWantToChangePassword) {
            setSuccessMessage("No changes were made");
            return;
        }

        if (userWantToChangePassword) {
            if (state.password.length < 1) {
                setErrorMessage("Current password cannot be empty");
                return;
            }

            if (state.newPassword.length > 0 && state.newPassword.length < 8) {
                setErrorMessage("Password must be at least 8 characters long");
                return;
            }

            if (state.newPassword !== state.confirmNewPassword) {
                setErrorMessage("Passwords do not match");
                return;
            }
        }

        api.updateAccount(
            state.name,
            state.email,
            state.password,
            state.newPassword
        )
            .then((response) => {
                if (response.status === 200) {
                    setSuccessMessage(
                        "Account updated successfully. You will be logged out in 5 seconds to apply the changes."
                    );
                    setTimeout(() => {
                        auth.userLogout();
                        navigate("/login");
                    }, 5000);
                }
            })
            .catch((error) => {
                console.error(error);
                if (error.response && error.response.status === 400) {
                    const errorMessage = JSON.stringify(error.response.data);
                    setErrorMessage(errorMessage);
                } else {
                    setErrorMessage(
                        "An error occurred while updating the account"
                    );
                }
            });
    };

    useEffect(() => {
        const fetchUserName = async () => {
            try {
                const user = await Promise.resolve(auth.getUser());
                const userName = user.name;
                const email = user.email;

                setState((prevState) => ({
                    ...prevState,
                    email: email,
                    name: userName,
                }));
            } catch (error) {
                console.error(error);
            }
        };

        fetchUserName();
    }, [auth]); // Maybe remove auth if errors occur

    return (
        <Container className='account'>
            <h1 className='text-center'>Update your account</h1>
            <Row className='justify-content-center'>
                <Col xs='12' sm='8' md='6' lg='4'>
                    <Form onSubmit={handleFormSubmit}>
                        <FormGroup>
                            <Label for='email'>E-Mail</Label>
                            <Input
                                type='text'
                                name='email'
                                id='email'
                                placeholder='E-Mail'
                                value={state.email}
                                onChange={handleInputChange}
                                readOnly={true} // Users cannot change their email
                                style={{ backgroundColor: "lightgray" }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for='name'>Name</Label>
                            <Input
                                type='text'
                                name='name'
                                id='name'
                                placeholder='Name'
                                value={state.name}
                                onChange={handleInputChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for='password'>Current Password</Label>
                            <Input
                                type='password'
                                name='password'
                                id='password'
                                placeholder='Current Password'
                                value={state.password}
                                onChange={handleInputChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for='newPassword'>New Password</Label>
                            <Input
                                type='password'
                                name='newPassword'
                                id='newPassword'
                                placeholder='New Password'
                                value={state.newPassword}
                                onChange={handleInputChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for='confirmNewPassword'>
                                Confirm New Password
                            </Label>
                            <Input
                                type='password'
                                name='confirmNewPassword'
                                id='confirmNewPassword'
                                placeholder='Confirm New Password'
                                value={state.confirmNewPassword}
                                onChange={handleInputChange}
                            />
                        </FormGroup>
                        <Button color='primary' block>
                            Update Account
                        </Button>
                    </Form>
                </Col>
            </Row>
            {successMessage && <Alert color='success'>{successMessage}</Alert>}
            {errorMessage && <Alert color='danger'>{errorMessage}</Alert>}
        </Container>
    );
};

export default AccountPage;
