// src/App.js
import React from 'react';
import CalendarView from './components/TaludiaCalendar';

function App() {
  return (
    <div className="App">
      <h1 style={{ textAlign: 'center' }}>Calendario Semanal</h1>
      <CalendarView />
    </div>
  );
}

export default App;
