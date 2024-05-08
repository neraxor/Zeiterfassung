import { useContext,FC } from 'react';
import { Link } from 'react-router-dom';
import { Navbar, Nav, NavItem } from 'reactstrap';
import { FaHourglass } from "react-icons/fa6";
import { AuthContext } from './AuthContext';
import { Button } from 'reactstrap';
import '../custom.css';

// CustomNavbar component
const CustomNavbar: FC = () => {
  const authContext = useContext(AuthContext);
  if (!authContext) {
    throw new Error('AuthContext is undefined');
  }
  const { isLoggedIn, logout } = authContext;  
  return (
    <div className="navbar-container"> 
      <Navbar color="light" light expand="md">
        <Link to="/" className="navbar-brand">
          <FaHourglass size={55} /><span className="navbar-logo-text">Zeiterfassung</span>
        </Link>
        <Nav className="ml-auto" navbar>
          <NavItem>
          </NavItem>
          {isLoggedIn && (
            <NavItem>
            <div className="navbar-item"> 
              <Button onClick={logout}>Logout</Button>
            </div>
            </NavItem>
          )}
        </Nav>
      </Navbar>
    </div>
  );
};
export default CustomNavbar;