import React, { useState, useEffect, useContext } from 'react';
import { Card, CardBody, CardHeader, ListGroup, ListGroupItem, Form, FormGroup, Label, Input, Col } from 'reactstrap';
import { AuthContext } from './AuthContext';

interface StatsData {
  averageStartTime: number;
  averageEndTime: number;
  weekdayAverages: { dayOfWeek: number, averageHours: number }[];
}

const getDayName = (dayNumber: number) => {
  const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
  return days[dayNumber % 7];
};

const StatisticsComponent: React.FC<{ token: string }> = ({ token }) => {
  const [stats, setStats] = useState<StatsData | null>(null);
  const [year, setYear] = useState(new Date().getFullYear());
  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const authContext = useContext(AuthContext);

  useEffect(() => {
    const fetchStats = async () => {
      const response = await fetch(`https://localhost:7249/WorkSession/GetMonthlyAverages?year=${year}&month=${month}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (response.ok) {
        const data: StatsData = await response.json();
        setStats(data);
      } else {
        const emptyStats: StatsData = {
          averageStartTime: 0,
          averageEndTime: 0,
          weekdayAverages: []
        };
        setStats(emptyStats);
        if (response.status === 401) {
          authContext?.logout();
        }
      }
    };
    fetchStats();
  }, [year, month, token]);
  if (!stats) return <div>Loading...</div>;
  return (
    <Card>
      <CardHeader>
        <h4>Statistics</h4>
        <Form>
          <FormGroup row>
            <Label for="year-select" sm={2}>Year:</Label>
            <Col sm={10}>
              <Input type="select" name="year" id="year-select" value={year} onChange={e => setYear(Number(e.target.value))}>
                {Array.from({ length: 5 }, (_, i) => new Date().getFullYear() - i).map(yr => (
                  <option key={yr} value={yr}>{yr}</option>
                ))}
              </Input>
            </Col>
          </FormGroup>
          <FormGroup row>
            <Label for="month-select" sm={2}>Month:</Label>
            <Col sm={10}>
              <Input type="select" name="month" id="month-select" value={month} onChange={e => setMonth(Number(e.target.value))}>
                {Array.from({ length: 12 }, (_, i) => i + 1).map(mo => (
                  <option key={mo} value={mo}>{mo}</option>
                ))}
              </Input>
            </Col>
          </FormGroup>
        </Form>
      </CardHeader>
      <CardBody>
        <ListGroup>
          <ListGroupItem>Average Start Time: {stats.averageStartTime.toFixed(2)}h</ListGroupItem>
          <ListGroupItem>Average End Time: {stats.averageEndTime.toFixed(2)}h</ListGroupItem>
          {stats.weekdayAverages.map((avg, idx) => (
            <ListGroupItem key={idx}>
              {getDayName(avg.dayOfWeek)}: Average {avg.averageHours.toFixed(2)} hours
            </ListGroupItem>
          ))}
        </ListGroup>
      </CardBody>
    </Card>
  );
};

export default StatisticsComponent;
