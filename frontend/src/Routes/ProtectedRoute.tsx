import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../Context/useAuth";

type Props = { children: React.ReactNode };

  const ProtectedRoute = ({ children }: Props) => 
  
  { // Component to protect routes that require authentication
    const location = useLocation();
    const { isLoggedIn } = useAuth();
    return isLoggedIn() ? (
                            <>{children}</>
                          ) : (
                                <Navigate to="/login" state={{ from: location }} replace />
                              );
  };

export default ProtectedRoute;
