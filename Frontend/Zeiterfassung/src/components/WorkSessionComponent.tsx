import React, { useState, useEffect, useContext } from 'react';
import { Row, Col, Button, Form, FormGroup, Input, Card, CardBody, CardHeader } from 'reactstrap';
import { AuthContext } from './AuthContext';
import moment from 'moment-timezone';

// WorkSession DTO
interface WorkSession {
  UserId: number | null;
  Start: Date;
  End: Date | null;
  LocationId: number | null;
  ProjectId: number | null;
}

// Project DTO
interface Project {
  id: string;
  name: string;
  description: string;
}

// Location DTO
interface Location {
  id: number;
  description: string;
  userId: number;
  user: null;
}

interface WorkSessionProps {
  token: string;
  onSessionSave: () => void;
}

// WorkSessionComponent
const WorkSessionComponent: React.FC<WorkSessionProps> = ({ token, onSessionSave }) => {
  const [workSession, setWorkSession] = useState<WorkSession>({
    UserId: null,
    Start: new Date(),
    End: null,
    LocationId: null,
    ProjectId: null
  });
  // Locations and Projects dropdown options
  const [locations, setLocations] = useState<Location[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  // Error and feedback messages
  const [error, setError] = useState('');
  const [feedback, setFeedback] = useState('');
  // set AuthContext
  const authContext = useContext(AuthContext);
  // useEffect hook to fetch dropdown options and current work session
  useEffect(() => {
    fetchDropdownOptions().then(() => {
      fetchCurrentWorkSession();
    });
  }, [token]);
  // fetchDropdownOptions function to fetch the dropdown options
  const fetchDropdownOptions = async () => {
    try {
      const locationResponse = await fetch('https://localhost:7249/User/GetLocations', {
        headers: { Authorization: `Bearer ${token}` }
      });
      const projectResponse = await fetch('https://localhost:7249/User/GetProjects', {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (locationResponse.status === 401 || projectResponse.status === 401) {
        authContext?.logout();
      }
      if (locationResponse.ok && projectResponse.ok) {
        const locationData = await locationResponse.json();
        const projectsData = await projectResponse.json();
        setLocations(locationData);
        setProjects(projectsData);
        setWorkSession(prev => ({
          ...prev,
          LocationId: locationData[0]?.id,
          ProjectId: projectsData[0]?.id
        }));
      }
    } catch (error) {
      console.error('Failed to fetch dropdown options:', error);
    }
  };
  // fetchCurrentWorkSession function to fetch the current work session
  const fetchCurrentWorkSession = async () => {
    try {
      const response = await fetch('https://localhost:7249/WorkSession/GetCurrentWorkSession', {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (response.status === 401) {
        authContext?.logout();
      }
      if (response.ok) {
        const currentWorkSession = await response.json();
        const localStart = moment.utc(currentWorkSession.start).tz('Europe/Berlin');
        const localEnd = moment.utc(currentWorkSession.End).tz('Europe/Berlin');
        setWorkSession({
          UserId: currentWorkSession.userId,
          Start: localStart.toDate(),
          End: localEnd ? localEnd.toDate() : null,
          LocationId: currentWorkSession.locationId,
          ProjectId: currentWorkSession.projectId
        });
      }
    } catch (error) {
      console.error('Failed to fetch current work session:', error);
    }
  };
  // handleInputChange function to handle input changes
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = event.target;
    setWorkSession(prev => ({ ...prev, [name]: value }));
  };
  // handleTime function to handle time changes
  const handleTime = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value: newTime } = event.target; 
    setWorkSession(prev => {
      const timeField = name === 'StartTime' ? prev.Start : (prev.End ?? new Date());
      const updatedDate = new Date(timeField);
  
      const [hours, minutes] = newTime.split(':').map(Number);
      updatedDate.setHours(hours, minutes); 
  
      return {
        ...prev,
        [name === 'StartTime' ? 'Start' : 'End']: updatedDate
      };
    });
  };
  // handleDate function to handle date changes
  const handleDate = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value: newDate } = event.target;
    setWorkSession(prev => {
      const dateField = name === 'StartDate' ? prev.Start : (prev.End ?? new Date());
      const updatedDate = new Date(dateField);

      const [year, month, day] = newDate.split('-').map(Number);
      updatedDate.setFullYear(year, month - 1, day);

      return {
        ...prev,
        [name === 'StartDate' ? 'Start' : 'End']: updatedDate
      };
    });
  };
  // handleSubmit function to handle form submission
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    try {
      const response = await fetch('https://localhost:7249/WorkSession/SaveWorkSession', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(workSession)
      });
      if (response.status === 401) {
        authContext?.logout();
      }
      if (response.ok) {
        setFeedback('WorkSession saved successfully.');
        setError('');
        console.log("userid", workSession.UserId);
        if (!workSession.UserId) {
          setFeedback('WorkSession saved successfully. Weiterleitung in 2 Sekunden...');
          setTimeout(() => {
            setFeedback('');
            fetchCurrentWorkSession();
          }, 2000);
        }
        onSessionSave();
      } else {
        setError('Failed to save WorkSession.');
        setFeedback('');
      }
      console.log('WorkSession saved successfully');
    } catch (error) {
      console.error('Failed to submit WorkSession:', error);
    }
  };

  const formatDate = (date: Date) => moment(date).format('YYYY-MM-DD');
  const formatTime = (date: Date) => moment(date).format('HH:mm');

  return (
    <Card>
      <CardHeader><h2>Work Session</h2></CardHeader>
      <CardBody>
        <Form onSubmit={handleSubmit}>
          <Row form>
            <Col md={6}>
              <FormGroup>
                <Input type="date" name="StartDate" value={formatDate(workSession.Start)} onChange={handleDate} />
              </FormGroup>
            </Col>
            <Col md={6}>
              <FormGroup>
                <Input type="time" name="StartTime" value={formatTime(workSession.Start)} onChange={handleTime} />
              </FormGroup>
            </Col>
          </Row>
          {workSession.UserId && (
            <Row form>
              <Col md={6}>
                <FormGroup>
                  <Input type="date" name="EndDate" id="EndDate" value={formatDate(workSession.End || new Date())} onChange={handleDate} />
                </FormGroup>
              </Col>
              <Col md={6}>
                <FormGroup>
                  <Input type="time" name="EndTime" id="EndTime" value={formatTime(workSession.End || new Date())} onChange={handleTime} />
                </FormGroup>
              </Col>
            </Row>
          )}
          <Row form>
            <Col md={6}>
              <FormGroup>
                <select name="LocationId" value={workSession.LocationId || ''} onChange={handleInputChange} className="form-control">
                  {locations.map((location) => (
                    <option key={location.id} value={location.id}>{location.description}</option>
                  ))}
                </select>
              </FormGroup>
            </Col>
            <Col md={6}>
              <FormGroup>
                <select name="ProjectId" value={workSession.ProjectId || ''} onChange={handleInputChange} className="form-control">
                  {projects.map((project) => (
                    <option key={project.id} value={project.id}>{project.name}</option>
                  ))}
                </select>
              </FormGroup>
            </Col>
          </Row>
          <Button type="submit">Save Session</Button>
          {error && <p>{error}</p>}
          {feedback && <p>{feedback}</p>}
        </Form>
      </CardBody>
    </Card>
  );
};

export default WorkSessionComponent;
