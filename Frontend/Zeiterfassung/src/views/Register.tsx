import  { useContext } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../custom.css';
import { Container, Row, Col } from 'reactstrap';
import { AuthContext } from '../components/AuthContext'; 
import Registercomponent from '../components/registerComponent';

//Register View initialiates the Registercomponent to register a new user
const Register = () => {
  const authContext = useContext(AuthContext); 

  return (
    <Container fluid>
      <Row>
        <Col md={6} className="mx-auto">
            <Registercomponent />
        </Col>
      </Row>
    </Container>
  );
};

export default Register;