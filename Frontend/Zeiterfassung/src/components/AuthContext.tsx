import React, { createContext, useEffect, useState, ReactNode } from 'react';

interface AuthContextType {
  // state indicating whether user is logged in or not
  isLoggedIn: boolean;
  // Function to set isLoggedIn state
  setIsLoggedIn: (isLoggedIn: boolean) => void;
  // User object containing username and email
  user: { username: string, email: string } | null;
  // Function to set user object
  setUser: (user: { username: string, email: string } | null) => void;
  // Function to login user
  login: () => void;
  // Function to logout user
  logout: () => void;
  // Function to get token from local storage
  getToken: () => string | null;
  tokenExists: boolean;
}
// Create context with default value
const AuthContext = createContext<AuthContextType | undefined>(undefined);
// Create provider that provides  auth-related data to its children
const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [user, setUser] = useState<{ username: string, email: string } | null>(null);
  const [tokenExists, setTokenExists] = useState(!!localStorage.getItem('jwt'));

  // Check if token exists in local storage
  useEffect(() => {
    const token = localStorage.getItem('jwt');
    if (token) {
      setIsLoggedIn(true);
      setTokenExists(true);
    }
  }, []);

  // Function to login user
  const login = () => {
    console.log("Login true set");
    setIsLoggedIn(true);
    setTokenExists(true);
  };

  // Function to logout user
  const logout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem('jwt');
    setTokenExists(false);
    console.log("Logout successful");
  };
  // Function to get token from local storage
  const getToken = () => {
    return localStorage.getItem('jwt');
  };
  return (
    <AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn, user, setUser, login, logout, getToken,tokenExists }}>
      {children}
    </AuthContext.Provider>
  );
};

export { AuthProvider, AuthContext };