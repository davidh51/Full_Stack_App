import axios from "axios";
import { CommentGet, CommentPost } from "../Models/Comment";
import { handleError } from "../Helpers/ErrorHandler";
import { Console } from "console";

//const api = "http://localhost:5230/api/comment/";
const api = process.env.REACT_APP_API_URL + "/comment/"; // Using environment variable for API URL, set in .env file

export const commentPostAPI = async (
  tittle: string,
  content: string,
  symbol: string
) => 
  {
  try {
    //const token = localStorage.getItem("token");  // Already in context!
    //console.log("Posting comment to API...!!!", tittle, content, api+symbol); //debugging
    const data = await axios.post<CommentPost>(api + symbol, 
    {
      tittle: tittle,
      content: content,
    });
    return data;
  } catch (error) {
    handleError(error);
  }
};

export const commentGetAPI = async (symbol: string) => {
  try {
    const data = await axios.get<CommentGet[]>(api + `?Symbol=${symbol}`);
    return data;
  } catch (error) {
    handleError(error);
  }
};
