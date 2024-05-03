import React, { useContext, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../custom.css';
import { Container, Row, Col } from 'reactstrap';
import { AuthContext } from '../components/AuthContext'; 
import Login from '../components/login';
import WorkSessionComponent from '../components/WorkSessionComponent';
import Stundenkonto from '../components/Stundenkonto';
import DownloadWorkSessions from '../components/downloadMonthlyWorkSessions';
import StatisticsComponent from '../components/StatisticsComponent';

const Home = () => {
  const authContext = useContext(AuthContext); 
  const [sessionUpdated, setSessionUpdated] = useState<boolean>(false);

  const handleSessionUpdate = () => {
    setSessionUpdated(true);
  };

  const resetSessionUpdate = () => {
    setSessionUpdated(false);
  };
  
  return (
    <Container fluid>
      <Row>
        <Col md={6} className="mx-auto">
          {!authContext?.isLoggedIn && <Login />}
          {authContext?.isLoggedIn && authContext.getToken() ? (
            <WorkSessionComponent token={authContext.getToken()!} onSessionSave={handleSessionUpdate} />
          ) : null}
          {authContext?.isLoggedIn && authContext.getToken() ? (
            <div style={{ marginTop: '20px' }}>
              <Stundenkonto token={authContext.getToken()!} sessionUpdated={sessionUpdated} resetUpdate={resetSessionUpdate} />
            </div>
          ) : null}
          <div style={{ marginTop: '20px' }}>
            {authContext?.isLoggedIn && authContext.getToken() ? <DownloadWorkSessions token={authContext.getToken()!} /> : null}
          </div>
          <div style={{ marginTop: '20px' }}>
            {authContext?.isLoggedIn && authContext.getToken() ? (
              <StatisticsComponent token={authContext.getToken()!} />
            ) : null}
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default Home;