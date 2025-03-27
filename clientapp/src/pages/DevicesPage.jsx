import React, { useEffect, useState } from 'react';
import Navbar from './Navbar';

const DevicesPage = () => {
    const [devices, setDevices] = useState([]);
    const [clients, setClients] = useState([]);
    const [loading, setLoading] = useState(true);
    const [editingDevice, setEditingDevice] = useState(null);

    const [name, setName] = useState('');
    const [devEui, setDevEui] = useState('');
    const [clientId, setClientId] = useState('');
    const [country, setCountry] = useState('');
    const [province, setProvince] = useState('');
    const [district, setDistrict] = useState('');

    const [editName, setEditName] = useState('');
    const [editCountry, setEditCountry] = useState('');
    const [editProvince, setEditProvince] = useState('');
    const [editDistrict, setEditDistrict] = useState('');

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
                    location: { country, province, district }
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

    const updateDeviceName = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/device/name', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({
                    devEui: editingDevice.devEui,
                    name: editName
                })
            });

            if (!res.ok) throw new Error('Update failed');

            setEditingDevice(null);
            fetchDevices();
        } catch (err) {
            console.error('Update error:', err);
            alert('Could not update device name.');
        }
    };

    const updateDeviceLocation = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/device/location', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({
                    devEui: editingDevice.devEui,
                    location: { country: editCountry, province: editProvince, district: editDistrict }
                })
            });

            if (!res.ok) {
                const errorData = await res.json();
                throw new Error(errorData.message || 'Update failed');
            }

            setEditingDevice(null);
            fetchDevices();
            alert('Location updated successfully!');
        } catch (err) {
            console.error('Update error:', err);
            alert(err.message || 'Could not update device location.');
        }
    };

    const deleteDevice = async (devEui) => {
        if (!window.confirm(`Delete device ${devEui}?`)) return;
        try {
            await fetch(`http://localhost:5000/api/device/${devEui}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` }
            });
            fetchDevices();
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

    const inputStyle = {
        backgroundColor: '#2c2c2c',
        color: '#fff',
        border: '1px solid #555',
        padding: '8px',
        borderRadius: '4px'
    };

    return (
        <div style={{ padding: '2rem', backgroundColor: '#121212', minHeight: '100vh', color: '#fff' }}>
            <Navbar />
            <h2>IoT Devices</h2>

            <div style={{
                marginBottom: '2rem',
                padding: '1rem',
                border: '1px solid #444',
                borderRadius: '4px',
                backgroundColor: '#1e1e1e'
            }}>
                <h3>Add New Device</h3>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px', alignItems: 'center' }}>
                    <input placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} style={inputStyle} />
                    <input placeholder="DevEUI" value={devEui} onChange={(e) => setDevEui(e.target.value)} style={inputStyle} />
                    <select value={clientId} onChange={(e) => setClientId(e.target.value)} style={inputStyle}>
                        <option value="">Select Client</option>
                        {clients.map((client) => (
                            <option key={client.id} value={client.id}>{client.name}</option>
                        ))}
                    </select>
                    <input placeholder="Country" value={country} onChange={(e) => setCountry(e.target.value)} style={inputStyle} />
                    <input placeholder="Province" value={province} onChange={(e) => setProvince(e.target.value)} style={inputStyle} />
                    <input placeholder="District" value={district} onChange={(e) => setDistrict(e.target.value)} style={inputStyle} />
                    <button onClick={addDevice} style={{
                        padding: '8px 16px',
                        backgroundColor: '#4CAF50',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px'
                    }}>
                        Add Device
                    </button>
                </div>
            </div>

            {editingDevice && (
                <div style={{
                    position: 'fixed',
                    top: 0, left: 0, right: 0, bottom: 0,
                    backgroundColor: 'rgba(0,0,0,0.7)',
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    zIndex: 1000
                }}>
                    <div style={{
                        backgroundColor: '#1e1e1e',
                        padding: '2rem',
                        borderRadius: '8px',
                        width: '500px',
                        color: '#fff'
                    }}>
                        <h3>Edit Device: {editingDevice.devEui}</h3>

                        <div style={{ marginBottom: '1rem' }}>
                            <h4>Update Name</h4>
                            <input value={editName} onChange={(e) => setEditName(e.target.value)} style={{ ...inputStyle, width: '100%' }} />
                            <button onClick={updateDeviceName} style={{
                                marginTop: '8px',
                                padding: '8px 16px',
                                backgroundColor: '#2196F3',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px'
                            }}>
                                Update Name
                            </button>
                        </div>

                        <div style={{ marginBottom: '1rem' }}>
                            <h4>Update Location</h4>
                            <input placeholder="Country" value={editCountry} onChange={(e) => setEditCountry(e.target.value)} style={{ ...inputStyle, width: '100%', marginBottom: '8px' }} />
                            <input placeholder="Province" value={editProvince} onChange={(e) => setEditProvince(e.target.value)} style={{ ...inputStyle, width: '100%', marginBottom: '8px' }} />
                            <input placeholder="District" value={editDistrict} onChange={(e) => setEditDistrict(e.target.value)} style={{ ...inputStyle, width: '100%', marginBottom: '8px' }} />
                            <button onClick={updateDeviceLocation} style={{
                                padding: '8px 16px',
                                backgroundColor: '#2196F3',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px'
                            }}>
                                Update Location
                            </button>
                        </div>

                        <button onClick={() => setEditingDevice(null)} style={{
                            padding: '8px 16px',
                            backgroundColor: '#f44336',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px'
                        }}>
                            Cancel
                        </button>
                    </div>
                </div>
            )}

            {loading ? (
                <p>Loading devices...</p>
            ) : (
                Object.keys(groupedDevices).map((clientName) => (
                    <div key={clientName} style={{ marginBottom: '2rem' }}>
                        <h3>Client: {clientName}</h3>
                        <table style={{ width: '100%', borderCollapse: 'collapse', backgroundColor: '#1e1e1e', color: '#ccc' }}>
                            <thead>
                                <tr style={{ backgroundColor: '#333' }}>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>DevEUI</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Name</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Location</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {groupedDevices[clientName].map((device) => (
                                    <tr key={device.devEui} style={{ borderBottom: '1px solid #444' }}>
                                        <td style={{ padding: '12px' }}>{device.devEui}</td>
                                        <td style={{ padding: '12px' }}>{device.name}</td>
                                        <td style={{ padding: '12px' }}>
                                            {device.location?.country}, {device.location?.province}, {device.location?.district}
                                        </td>
                                        <td style={{ padding: '12px' }}>
                                            <button onClick={() => {
                                                setEditingDevice(device);
                                                setEditName(device.name);
                                                setEditCountry(device.location?.country || '');
                                                setEditProvince(device.location?.province || '');
                                                setEditDistrict(device.location?.district || '');
                                            }} style={{
                                                padding: '6px 12px',
                                                backgroundColor: '#2196F3',
                                                color: 'white',
                                                border: 'none',
                                                marginRight: '8px',
                                                borderRadius: '4px'
                                            }}>
                                                Edit
                                            </button>
                                            <button onClick={() => deleteDevice(device.devEui)} style={{
                                                padding: '6px 12px',
                                                backgroundColor: '#f44336',
                                                color: 'white',
                                                border: 'none',
                                                borderRadius: '4px'
                                            }}>
                                                Delete
                                            </button>
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
