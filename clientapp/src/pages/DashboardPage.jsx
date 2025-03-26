import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const DashboardPage = () => {
    const [clientCount, setClientCount] = useState(0);
    const [deviceCount, setDeviceCount] = useState(0);
    const [recentDevices, setRecentDevices] = useState([]);
    const navigate = useNavigate();

    const token = localStorage.getItem('token');

    useEffect(() => {
        const fetchData = async () => {
            try {
                // Clients
                const clientRes = await fetch('http://localhost:5000/api/client', {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                const clients = await clientRes.json();
                setClientCount(clients.length);

                // Devices
                const deviceRes = await fetch('http://localhost:5000/api/device', {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                const devices = await deviceRes.json();
                setDeviceCount(devices.length);

                // Show last 5
                setRecentDevices(devices.slice(-5).reverse());
            } catch (err) {
                console.error('Error fetching dashboard data:', err);
            }
        };

        fetchData();
    }, [token]);

    return (
        <div style={{ padding: '2rem' }}>
            <h2>Welcome to Plan-S Dashboard</h2>

            <div style={{ marginBottom: '2rem' }}>
                <h3>Total Clients: {clientCount}</h3>
                <h3>Total Devices: {deviceCount}</h3>
            </div>

            <div style={{ marginBottom: '2rem' }}>
                <button onClick={() => navigate('/clients')}>Manage Clients</button>
                <button onClick={() => navigate('/devices')} style={{ marginLeft: '10px' }}>Manage Devices</button>
                <button onClick={() => navigate('/data')} style={{ marginLeft: '10px' }}>Query IoT Data</button>
            </div>

            <div>
                <h3>Recent Devices</h3>
                <ul>
                    {recentDevices.map((d) => (
                        <li key={d.devEui}>{d.name} - {d.devEui}</li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default DashboardPage;
