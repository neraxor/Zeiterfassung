import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import NavBar from './components/CustomNavBar';
import Footer from './components/CustomFooter';
import Home from './views/Home';
import { AuthProvider } from './components/AuthContext';
import Register from './views/Register';
import './App.css';

function App() {
  return (
    <AuthProvider>  
      <Router>
        <div className="App">
          <NavBar />
          <Routes> 
            <Route path="/" element={<Home />} />
            <Route path="/home" element={<Home />} />
            <Route path="/register" element={<Register/>} />
          </Routes>
          <Footer />
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
