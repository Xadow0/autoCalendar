// src/api.js
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

export async function fetchEvents(date) {
  const response = await fetch(`${API_BASE_URL}/calendar/events?date=${date}`);
  if (!response.ok) {
    const text = await response.text();
    throw new Error(`Error del servidor: ${text}`);
  }
  return await response.json();
}
