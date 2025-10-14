import { createContext, useEffect, useState } from "react";
import { UserProfile } from "../Models/User";
import { useNavigate } from "react-router-dom";
import { loginAPI, registerAPI } from "../Services/AuthService";
import { toast } from "react-toastify";
import React from "react";
import axios from "axios";

type UserContextType = { // Define the shape of the user context
                          user: UserProfile | null;
                          token: string | null;
                          registerUser: (email: string, username: string, password: string) => void;
                          loginUser: (username: string, password: string) => void;
                          logout: () => void;
                          isLoggedIn: () => boolean;
                        };

type Props = { children: React.ReactNode }; // Props type for the provider component

const UserContext = createContext<UserContextType>({} as UserContextType);

export const UserProvider = ({ children }: Props) => 
{

  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {  // Check for token and user in local storage on component mount
                    const user = localStorage.getItem("user");
                    const token = localStorage.getItem("token");
                    if (user && token) {
                                          setUser(JSON.parse(user));
                                          setToken(token);
                                          axios.defaults.headers.common["Authorization"] = "Bearer " + token;
                                        }
                    setIsReady(true);
                  }, []);

  const registerUser = async ( // Register a new user
                                email: string,
                                username: string,
                                password: string
                              ) => 
    {
      await registerAPI(email, username, password).then((res) => // Call register API
          { 
            if (res) {
                      localStorage.setItem("token", res?.data.token);
                      const userObj = 
                      {
                        userName: res?.data.userName,
                        email: res?.data.email,
                      };
                      localStorage.setItem("user", JSON.stringify(userObj));
                      setToken(res?.data.token!);
                      setUser(userObj!);
                      toast.success("Login Success!");
                      navigate("/search");
                    }
          })
            .catch((e) => toast.warning("Server error occured"));
    };

  const loginUser = async (
                            username: string, 
                            password: string
                          ) => 
    {
      await loginAPI(username, password).then((res) =>  // Call login API
        {
          if (res) {
                    localStorage.setItem("token", res?.data.token);
                    const userObj = {
                      userName: res?.data.userName,
                      email: res?.data.email,
                    };
                    localStorage.setItem("user", JSON.stringify(userObj));
                    setToken(res?.data.token!);
                    setUser(userObj!);
                    toast.success("Login Success!");
                    navigate("/search");
                    }
        })
          .catch((e) => toast.warning("Server error occured"));
      };

  const isLoggedIn = () => { // Check if user is logged in
    return !!user;
  };

  const logout = () => // Logout the user
    {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      setUser(null);
      setToken("");
      navigate("/");
    };

  return ( // Provide user context to children components!! - returns the context provider
            <UserContext.Provider
              value={{ loginUser, user, token, logout, isLoggedIn, registerUser }}
            >
              {isReady ? children : null} 
            </UserContext.Provider> // async initialization 
          );
};

export const useAuth = () => React.useContext(UserContext); // Custom hook to use the UserContext
