import { Link } from 'react-router-dom';
import { Nav, NavItem, NavLink } from 'reactstrap';
import '../custom.css';

const CustomFooter = () => {
  return (
    <footer className="footer">
      <Nav className="justify-content-center">
        {/* todo */}
        <NavItem>
          <NavLink tag={Link} to="/impressum">Impressum</NavLink>
        </NavItem>
        <NavItem>
          <NavLink tag={Link} to="/datenschutz">Datenschutz</NavLink>
        </NavItem>
        <NavItem>
          <NavLink tag={Link} to="/kontakt">Kontakt</NavLink>
        </NavItem>
      </Nav>
      <div className="footer-text">
        <p>© 2024 Zeiterfassung. Alle Rechte vorbehalten.</p>
      </div>
    </footer>
  );
};

export default CustomFooter;
