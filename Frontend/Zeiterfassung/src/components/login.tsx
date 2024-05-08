import React, { useContext, useState, FormEvent, ChangeEvent } from 'react';
import { AuthContext } from './AuthContext';
import { Row, Col, Button, Form, FormGroup, Input } from 'reactstrap';
import { Link } from 'react-router-dom';

//Login component
const Login: React.FC = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const authContext = useContext(AuthContext);

  if (!authContext) {
    throw new Error('AuthContext is undefined');
  }
  //handleLogin function with API call
  const handleLogin = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const loginDetails = {
      Username: username,
      Password: password,
      Email: email
    };
    try {
      const response = await fetch('https://localhost:7249/api/Auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: JSON.stringify(loginDetails),
        credentials: 'include'
      });
      if (response.ok) {
        const responseData = await response.json();
        localStorage.setItem('jwt', responseData);
        authContext.login();
      } else {
        throw new Error('Login fehlgeschlagen');
      }
    } catch (error) {
      console.error('Login fehlgeschlagen:', error);
    }
  };

  //handleUsernameChange function to set the username
  const handleUsernameChange = (event: ChangeEvent<HTMLInputElement>) => {
    setUsername(event.target.value);
  };

  //handlePasswordChange function to set the password
  const handlePasswordChange = (event: ChangeEvent<HTMLInputElement>) => {
    setPassword(event.target.value);
  };

  return (
    <div>
      <Form onSubmit={handleLogin}>
        <h1>Login</h1>
        <Row form>
          <Col md={12}>
            <FormGroup>
              <Input type="text" value={username} onChange={handleUsernameChange} placeholder="Benutzername" />
            </FormGroup>
          </Col>
        </Row>
        <Row form>
          <Col md={12}>
            <FormGroup>
              <Input type="password" value={password} onChange={handlePasswordChange} placeholder="Passwort" />
            </FormGroup>
          </Col>
        </Row>
        <Row form>
          <Col md={12}>
            <FormGroup>
              <Button type="submit">Login</Button>
            </FormGroup>
            <Button tag={Link} to="/register">Registrieren</Button>
          </Col>
        </Row>
      </Form>
    </div>
  );
};

export default Login;
