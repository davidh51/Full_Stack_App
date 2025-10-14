import axios from "axios";
import { PortfolioGet, PortfolioPost } from "../Models/Portfolio";
import { handleError } from "../Helpers/ErrorHandler";

//const api = "http://localhost:5230/api/portfolio/";
const api = process.env.REACT_APP_API_URL + "/portfolio/"; // Using env variable for API URL (see .env file)

export const portfolioAddAPI = async (symbol: string) => {
  try {
    //console.log("Posting comment to API...!!!", symbol); //debugging
    const data = await axios.post<PortfolioPost>(api + symbol);
    return data;
  } catch (error) {
    handleError(error);
  }
};

export const portfolioDeleteAPI = async (symbol: string) => {
  try {
    //console.log("Posting comment to API...!!!",api, symbol); //debugging
    const data = await axios.delete<PortfolioPost>(api, {
                                                        headers: // override for this call, otherwise 415 error on delete calls
                                                        {
                                                        "Content-Type": "application/json", // override for this call
                                                        },
                                                        data : JSON.stringify(symbol)
                                                        });
    return data;
  } catch (error) {
    handleError(error);
  }
};

export const portfolioGetAPI = async () => {
  try {
    const data = await axios.get<PortfolioGet[]>(api);
    return data;
  } catch (error) {
    handleError(error);
  }
};
