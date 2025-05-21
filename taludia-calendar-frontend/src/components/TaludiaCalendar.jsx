// src/components/TaludiaCalendar.jsx
import React, { useState, useEffect } from 'react';
import { fetchEvents } from '../api';
import dayjs from 'dayjs';

const TaludiaCalendar = () => {
  const [inputDate, setInputDate] = useState(dayjs().format('YYYY-MM-DD')); // Fecha del input
  const [selectedDate, setSelectedDate] = useState(null); // Fecha seleccionada al hacer clic en "Cargar semana"
  const [events, setEvents] = useState([]);
  const [weekDays, setWeekDays] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const [obras, setObras] = useState([]);
  const [trabajadores, setTrabajadores] = useState([]);
  const [vehiculos, setVehiculos] = useState([]);

  useEffect(() => {
    if (!selectedDate) return;
    const startOfWeek = dayjs(selectedDate).startOf('week').add(1, 'day'); // lunes
    const days = Array.from({ length: 7 }, (_, i) =>
      startOfWeek.add(i, 'day').format('YYYY-MM-DD')
    );
    setWeekDays(days);
  }, [selectedDate]);

  const parseTitle = (title) => {
    const obra = title?.match(/^(.*?)\s*(\(|\[|$)/)?.[1]?.trim() ?? '';
    const trabajadores = title?.match(/\(([^)]*)\)/)?.[1]?.split(',').map(t => t.trim()) ?? [];
    const vehiculos = title?.match(/\[([^\]]*)\]/)?.[1]?.split(',').map(v => v.trim()) ?? [];
    return { obra, trabajadores, vehiculos };
  };

  const loadEvents = async () => {
    setSelectedDate(inputDate); // Esto activará el useEffect y mostrará la semana
    setLoading(true);
    setError(null);

    try {
      const data = await fetchEvents(inputDate);
      setEvents(data);

      // Extraer elementos únicos
      const obrasSet = new Set();
      const trabajadoresSet = new Set();
      const vehiculosSet = new Set();

      data.forEach(ev => {
        const { obra, trabajadores, vehiculos } = parseTitle(ev.title);
        if (obra) obrasSet.add(obra);
        trabajadores.forEach(t => t && trabajadoresSet.add(t));
        vehiculos.forEach(v => v && vehiculosSet.add(v));
      });

      setObras(Array.from(obrasSet));
      setTrabajadores(Array.from(trabajadoresSet));
      setVehiculos(Array.from(vehiculosSet));
    } catch (err) {
      setError('No se pudieron cargar los eventos.');
    } finally {
      setLoading(false);
    }
  };

  const groupEventsByDay = () => {
    const grouped = {};
    weekDays.forEach(day => grouped[day] = []);

    events.forEach(event => {
      const date = dayjs(event.start).format('YYYY-MM-DD');
      if (grouped[date]) {
        grouped[date].push(event);
      }
    });

    return grouped;
  };

  const groupedEvents = groupEventsByDay();

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      {/* Encabezado */}
      <div className="flex items-center justify-between gap-4">
        <input
          type="date"
          value={inputDate}
          onChange={(e) => setInputDate(e.target.value)}
          className="border p-2 rounded"
        />
        <button
          onClick={loadEvents}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          disabled={loading}
        >
          {loading ? 'Cargando...' : 'Cargar semana'}
        </button>
      </div>

      {/* Estados */}
      {error && <div className="text-red-500">{error}</div>}

      {/* Calendario semanal */}
      {selectedDate && (
        <div className="overflow-x-auto">
          <table className="min-w-full border text-sm text-left">
            <thead className="bg-gray-100">
              <tr>
                {weekDays.map(day => (
                  <th key={day} className="p-2 border">
                    {dayjs(day).format('dddd D MMM')}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              <tr>
                {weekDays.map(day => (
                  <td key={day} className="align-top p-2 border h-40 w-48">
                    {groupedEvents[day]?.map((ev, idx) => {
                      const parsed = parseTitle(ev.title);
                      return (
                        <div key={idx} className="bg-blue-100 mb-2 p-2 rounded shadow-sm">
                          <div className="font-semibold">{parsed.obra}</div>
                          <div className="text-xs text-gray-700">{parsed.trabajadores.join(', ')}</div>
                          <div className="text-xs text-gray-500 italic">{parsed.vehiculos.join(', ')}</div>
                        </div>
                      );
                    })}
                  </td>
                ))}
              </tr>
            </tbody>
          </table>
        </div>
      )}

      {/* Listas separadas */}
      {selectedDate && (
        <div className="grid grid-cols-3 gap-6">
          <div>
            <h3 className="text-lg font-bold mb-2">Obras</h3>
            <ul className="bg-white border rounded p-2 max-h-64 overflow-y-auto space-y-1">
              {obras.map((obra, idx) => (
                <li key={idx} className="p-1 hover:bg-gray-100 cursor-pointer">{obra}</li>
              ))}
            </ul>
          </div>
          <div>
            <h3 className="text-lg font-bold mb-2">Trabajadores</h3>
            <ul className="bg-white border rounded p-2 max-h-64 overflow-y-auto space-y-1">
              {trabajadores.map((t, idx) => (
                <li key={idx} className="p-1 hover:bg-gray-100 cursor-pointer">{t}</li>
              ))}
            </ul>
          </div>
          <div>
            <h3 className="text-lg font-bold mb-2">Vehículos</h3>
            <ul className="bg-white border rounded p-2 max-h-64 overflow-y-auto space-y-1">
              {vehiculos.map((v, idx) => (
                <li key={idx} className="p-1 hover:bg-gray-100 cursor-pointer">{v}</li>
              ))}
            </ul>
          </div>
        </div>
      )}
    </div>
  );
};

export default TaludiaCalendar;
