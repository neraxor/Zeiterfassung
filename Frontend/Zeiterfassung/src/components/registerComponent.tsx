import React, { useState, ChangeEvent, FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { Form, FormGroup, Input, Button, Row, Col } from 'reactstrap';
import { useNavigate } from 'react-router-dom';

//Register component
const Register: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isRegistered, setIsRegistered] = useState(false);
  const [isFailed, setIsFailed] = useState(false);
  const [error, setError] = useState('');
  const [email, setEmail] = useState('');

  //handleUsernameChange function to set the username
  const handleUsernameChange = (event: ChangeEvent<HTMLInputElement>) => {
    setUsername(event.target.value);
  };

  //handlePasswordChange function to set the password
  const handlePasswordChange = (event: ChangeEvent<HTMLInputElement>) => {
    setPassword(event.target.value);
  };

  //handleEmailChange function to set the email
  const handleEmailChange = (event: ChangeEvent<HTMLInputElement>) => {
    setEmail(event.target.value);
  };

  //useNavigate hook to navigate back to home after registration
  const navigate = useNavigate();
  
  //handleRegister function with API call
  const handleRegister = async (event: FormEvent) => {
    event.preventDefault();

    const response = await fetch('https://localhost:7249/api/Auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email,
        password: password,
        username: username
      })
    });
    if (response.ok) {
      setIsRegistered(true);
      setIsFailed(false);
      setError('');
      setTimeout(() => {
        navigate('/');
      }, 5000);
    } else {
      if (!response.ok) {
        const body = await response.clone().text();
        try {
          const data = JSON.parse(body);
          if (data.errors) {
            const errorMessages = Object.values(data.errors).flat().join(', ');
            setError(errorMessages);
          } else {
            setError(data.title || 'Ein unbekannter Fehler ist aufgetreten');
          }
        } catch (jsonError) {
          setError(body);
        }
      }
    }
  };

  return (
    <div>
      <Form onSubmit={handleRegister}>
        <h1>Registrierung</h1>
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
              <Input type="email" value={email} onChange={handleEmailChange} placeholder="Email" />
            </FormGroup>
          </Col>
        </Row>
        <Row form>
          <div>
            {isRegistered && <p>Registrierung erfolgreich! Sie werden zur Login-Seite weitergeleitet.</p>}
          </div>
          <div>
            {isFailed && <p>Registrierung fehlgeschlagen!</p>}
            <p>{error}</p>
          </div>
          <Col md={12}>
            <FormGroup>
              <Button tag={Link} to="/">Login</Button>

            </FormGroup>
            <FormGroup>
              <Button type="submit">Registrieren</Button>
            </FormGroup>
          </Col>
        </Row>
      </Form>
    </div>
  );
};

export default Register;