import React, { useState, useEffect, useContext } from 'react';
import { Card, CardBody, CardHeader, Progress } from 'reactstrap';
import { AuthContext } from './AuthContext';

interface StundenkontoProps {
  token: string;
  sessionUpdated: boolean;
  resetUpdate: () => void;
}
//Stundenkonto component
const Stundenkonto: React.FC<StundenkontoProps> = ({ token, sessionUpdated, resetUpdate }) => {
  const [totalHours, setTotalHours] = useState(40);
  const [workedHours, setWorkedHours] = useState(0);
  const [remainingHours, setRemainingHours] = useState(0);
  const [overHours, setOverHours] = useState(0);
  const [hasOverTime, setHasOverTime] = useState(false);
  const authContext = useContext(AuthContext);

  useEffect(() => {
    fetchWorkedHours();
  }, []);
  //useEffect hook to calculate the remaining hours and over hours
  useEffect(() => {
    const calculatedRemaining = parseFloat((totalHours - workedHours).toFixed(2));
    if (calculatedRemaining < 0) {
      setHasOverTime(true);
      setOverHours(Math.abs(calculatedRemaining));
      setRemainingHours(0);
    } else {
      setHasOverTime(false);
      setRemainingHours(calculatedRemaining);
      setOverHours(0);
    }
    if (sessionUpdated) {
      fetchWorkedHours();
      resetUpdate();
    }
  }, [totalHours, workedHours, sessionUpdated, resetUpdate]);
  //fetchWorkedHours function to fetch the worked hours
  const fetchWorkedHours = async () => {
    try {
      const response = await fetch('https://localhost:7249/WorkSession/GetWeeklyWorkHours', {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (response.status === 401) {
        authContext?.logout();
      }
      if (response.ok) {
        const data = await response.json();
        setTotalHours(data.workingHoursWeekly);
        setWorkedHours(data.workedHours);
      } else {
        console.error('Failed to fetch worked hours');
      }
    } catch (error) {
      console.error('Error fetching worked hours:', error);
    }
  };

  const progressValue = Math.min(100, (workedHours / totalHours) * 100);
  const overProgressValue = Math.max(0, (workedHours - totalHours) / totalHours * 100);

  return (
    <Card>
      <CardHeader><h4>Stundenübersicht</h4></CardHeader>
      <CardBody>
        <div>Gearbeitete Stunden: {workedHours}h</div>
        {hasOverTime ? (
          <div>Überstunden: {overHours}h</div>
        ) : (
          <div>Verbleibende Stunden: {remainingHours}h</div>
        )}
        <Progress multi>
          <Progress bar color="success" value={progressValue} />
          {hasOverTime && (
            <Progress bar color="danger" value={overProgressValue} />
          )}
        </Progress>
      </CardBody>
    </Card>
  );
};

export default Stundenkonto;
