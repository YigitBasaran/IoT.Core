import React, { useEffect, useState } from 'react';

const DevicesPage = () => {
    const [devices, setDevices] = useState([]);
    const [clients, setClients] = useState([]);
    const [loading, setLoading] = useState(true);

    const [name, setName] = useState('');
    const [devEui, setDevEui] = useState('');
    const [clientId, setClientId] = useState('');
    const [country, setCountry] = useState('');
    const [province, setProvince] = useState('');
    const [district, setDistrict] = useState('');

    const token = localStorage.getItem('token');

    const fetchDevices = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/device', {
                headers: { Authorization: `Bearer ${token}` }
            });
            const data = await res.json();
            setDevices(data);
        } catch (err) {
            console.error('Failed to fetch devices:', err);
        }
    };

    const fetchClients = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/client', {
                headers: { Authorization: `Bearer ${token}` }
            });
            const data = await res.json();
            setClients(data);
        } catch (err) {
            console.error('Failed to fetch clients:', err);
        }
    };

    const addDevice = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/device', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({
                    name,
                    devEui,
                    clientId: parseInt(clientId),
                    location: {
                        country,
                        province,
                        district
                    }
                })
            });

            if (!res.ok) throw new Error('Device creation failed');

            setName('');
            setDevEui('');
            setClientId('');
            setCountry('');
            setProvince('');
            setDistrict('');
            fetchDevices();
        } catch (err) {
            console.error('Add device error:', err);
            alert('Could not add device.');
        }
    };

    const deleteDevice = async (devEui) => {
        if (!window.confirm(`Delete device ${devEui}?`)) return;

        try {
            await fetch(`http://localhost:5000/api/device/${devEui}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` }
            });
            setDevices((prev) => prev.filter((d) => d.devEui !== devEui));
        } catch (err) {
            console.error('Delete error:', err);
        }
    };

    useEffect(() => {
        Promise.all([fetchDevices(), fetchClients()]).then(() => setLoading(false));
    }, []);

    const groupedDevices = devices.reduce((acc, device) => {
        const client = clients.find((c) => c.id === device.clientId);
        const clientName = client?.name || 'Unknown';
        acc[clientName] = acc[clientName] || [];
        acc[clientName].push(device);
        return acc;
    }, {});

    return (
        <div style={{ padding: '2rem' }}>
            <h2>IoT Devices</h2>

            <div style={{ marginBottom: '1rem' }}>
                <input placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} />
                <input placeholder="DevEUI" value={devEui} onChange={(e) => setDevEui(e.target.value)} style={{ marginLeft: '10px' }} />
                <select value={clientId} onChange={(e) => setClientId(e.target.value)} style={{ marginLeft: '10px' }}>
                    <option value="">Select Client</option>
                    {clients.map((client) => (
                        <option key={client.id} value={client.id}>{client.name}</option>
                    ))}
                </select>
                <input placeholder="Country" value={country} onChange={(e) => setCountry(e.target.value)} style={{ marginLeft: '10px' }} />
                <input placeholder="Province" value={province} onChange={(e) => setProvince(e.target.value)} style={{ marginLeft: '10px' }} />
                <input placeholder="District" value={district} onChange={(e) => setDistrict(e.target.value)} style={{ marginLeft: '10px' }} />
                <button onClick={addDevice} style={{ marginLeft: '10px' }}>Add Device</button>
            </div>

            {loading ? (
                <p>Loading devices...</p>
            ) : (
                Object.keys(groupedDevices).map((clientName) => (
                    <div key={clientName} style={{ marginBottom: '2rem' }}>
                        <h3>Client: {clientName}</h3>
                        <table border="1" cellPadding="8" style={{ borderCollapse: 'collapse', width: '100%' }}>
                            <thead>
                                <tr>
                                    <th>DevEUI</th>
                                    <th>Name</th>
                                    <th>Location</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {groupedDevices[clientName].map((device) => (
                                    <tr key={device.devEui}>
                                        <td>{device.devEui}</td>
                                        <td>{device.name}</td>
                                        <td>{`${device.location?.country}, ${device.location?.province}, ${device.location?.district}`}</td>
                                        <td>
                                            <button onClick={() => deleteDevice(device.devEui)}>Delete</button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ))
            )}
        </div>
    );
};

export default DevicesPage;
