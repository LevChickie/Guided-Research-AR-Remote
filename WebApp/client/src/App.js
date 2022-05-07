import React from 'react';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import CreateRoom from "./routes/CreateRoom";
import Room from "./routes/Room";
import './App.css';

function App() {
  return (
      <BrowserRouter>
        <Routes>
          <Route path="/" exact element={<CreateRoom/>} />
          <Route path="/room/:roomID" element={<Room/>} />
        </Routes> 
      </BrowserRouter>
  );
}

export default App;