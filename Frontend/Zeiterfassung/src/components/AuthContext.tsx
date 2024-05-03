import React, { createContext, useEffect, useState, ReactNode } from 'react';

interface AuthContextType {
  isLoggedIn: boolean;
  setIsLoggedIn: (isLoggedIn: boolean) => void;
  user: { username: string, email: string } | null;
  setUser: (user: { username: string, email: string } | null) => void;
  login: () => void;
  logout: () => void;
  getToken: () => string | null;
  tokenExists: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);
const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [user, setUser] = useState<{ username: string, email: string } | null>(null);
  const [tokenExists, setTokenExists] = useState(!!localStorage.getItem('jwt'));

  useEffect(() => {
    const token = localStorage.getItem('jwt');
    if (token) {
      setIsLoggedIn(true);
      setTokenExists(true);
    }
  }, []);

  const login = () => {
    console.log("Login true set");
    setIsLoggedIn(true);
    setTokenExists(true);
  };

  const logout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem('jwt');
    setTokenExists(false);
    console.log("Logout successful");
  };
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