import React, { useState } from 'react';
import { Button, Col, Form, FormGroup, Input, Label, Row, Card, CardBody, CardHeader } from 'reactstrap';

interface DownloadWorkSessionsProps {
  token: string;
}

//DownloadWorkSessions component
const DownloadWorkSessions: React.FC<DownloadWorkSessionsProps> = ({ token }) => {
  const [year, setYear] = useState<number>(new Date().getFullYear());
  const [month, setMonth] = useState<number>(new Date().getMonth() + 1);

  //downloadMonthlyWorkSessions function with API call
  const downloadMonthlyWorkSessions = async () => {
    const response = await fetch(`https://localhost:7249/WorkSession/DownloadMonthlyWorkSessions?year=${year}&month=${month}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'text/csv'
      }
    });
    if (response.ok) {
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `WorkSessions-${year}-${month}.csv`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } else {
      console.error('Failed to download file');
    }
  };

  return (
    <Card>
      <CardHeader><h4>Download Monthly Work Sessions</h4></CardHeader>
      <CardBody>
        <Form>
          <Row form>
            <Col md={6}>
              <FormGroup>
                <Label for="year-select">Year:</Label>
                <Input type="select" name="year" id="year-select" value={year} onChange={e => setYear(Number(e.target.value))}>
                  {Array.from({ length: 10 }).map((_, idx) => (
                    <option key={idx} value={new Date().getFullYear() - idx}>
                      {new Date().getFullYear() - idx}
                    </option>
                  ))}
                </Input>
              </FormGroup>
            </Col>
            <Col md={6}>
              <FormGroup>
                <Label for="month-select">Month:</Label>
                <Input type="select" name="month" id="month-select" value={month} onChange={e => setMonth(Number(e.target.value))}>
                  {Array.from({ length: 12 }).map((_, idx) => (
                    <option key={idx} value={idx + 1}>
                      {idx + 1}
                    </option>
                  ))}
                </Input>
              </FormGroup>
            </Col>
          </Row>
          <Button onClick={downloadMonthlyWorkSessions}>Download CSV</Button>
        </Form>
      </CardBody>
    </Card>
  );
};

export default DownloadWorkSessions;
