import React, { useEffect, useState } from 'react';
import Navbar from './Navbar';

const DevicesPage = () => {
    const [devices, setDevices] = useState([]);
    const [clients, setClients] = useState([]);
    const [loading, setLoading] = useState(true);
    const [editingDevice, setEditingDevice] = useState(null);

    // Add device form state
    const [name, setName] = useState('');
    const [devEui, setDevEui] = useState('');
    const [clientId, setClientId] = useState('');
    const [country, setCountry] = useState('');
    const [province, setProvince] = useState('');
    const [district, setDistrict] = useState('');

    // Edit forms state
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
                    location: {
                        country,
                        province,
                        district
                    }
                })
            });

            if (!res.ok) throw new Error('Device creation failed');

            // Reset form
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
                    location: {  // Note the nested location object
                        country: editCountry,
                        province: editProvince,
                        district: editDistrict
                    }
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

    return (
        <div style={{ padding: '2rem' }}>
            <Navbar />
            <h2>IoT Devices</h2>

            {/* Add Device Form */}
            <div style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid #ddd', borderRadius: '4px' }}>
                <h3>Add New Device</h3>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px', alignItems: 'center' }}>
                    <input placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} />
                    <input placeholder="DevEUI" value={devEui} onChange={(e) => setDevEui(e.target.value)} />
                    <select value={clientId} onChange={(e) => setClientId(e.target.value)}>
                        <option value="">Select Client</option>
                        {clients.map((client) => (
                            <option key={client.id} value={client.id}>{client.name}</option>
                        ))}
                    </select>
                    <input placeholder="Country" value={country} onChange={(e) => setCountry(e.target.value)} />
                    <input placeholder="Province" value={province} onChange={(e) => setProvince(e.target.value)} />
                    <input placeholder="District" value={district} onChange={(e) => setDistrict(e.target.value)} />
                    <button onClick={addDevice} style={{ padding: '8px 16px', backgroundColor: '#4CAF50', color: 'white', border: 'none' }}>
                        Add Device
                    </button>
                </div>
            </div>

            {/* Edit Modal */}
            {editingDevice && (
                <div style={{
                    position: 'fixed',
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    backgroundColor: 'rgba(0,0,0,0.5)',
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    zIndex: 1000
                }}>
                    <div style={{
                        backgroundColor: 'white',
                        padding: '2rem',
                        borderRadius: '8px',
                        width: '500px'
                    }}>
                        <h3>Edit Device: {editingDevice.devEui}</h3>

                        <div style={{ marginBottom: '1rem' }}>
                            <h4>Update Name</h4>
                            <input
                                value={editName}
                                onChange={(e) => setEditName(e.target.value)}
                                style={{ width: '100%', padding: '8px' }}
                            />
                            <button
                                onClick={updateDeviceName}
                                style={{
                                    marginTop: '8px',
                                    padding: '8px 16px',
                                    backgroundColor: '#2196F3',
                                    color: 'white',
                                    border: 'none'
                                }}
                            >
                                Update Name
                            </button>
                        </div>

                        <div style={{ marginBottom: '1rem' }}>
                            <h4>Update Location</h4>
                            <input
                                placeholder="Country"
                                value={editCountry}
                                onChange={(e) => setEditCountry(e.target.value)}
                                style={{ width: '100%', padding: '8px', marginBottom: '8px' }}
                            />
                            <input
                                placeholder="Province"
                                value={editProvince}
                                onChange={(e) => setEditProvince(e.target.value)}
                                style={{ width: '100%', padding: '8px', marginBottom: '8px' }}
                            />
                            <input
                                placeholder="District"
                                value={editDistrict}
                                onChange={(e) => setEditDistrict(e.target.value)}
                                style={{ width: '100%', padding: '8px', marginBottom: '8px' }}
                            />
                            <button
                                onClick={updateDeviceLocation}
                                style={{
                                    padding: '8px 16px',
                                    backgroundColor: '#2196F3',
                                    color: 'white',
                                    border: 'none'
                                }}
                            >
                                Update Location
                            </button>
                        </div>

                        <button
                            onClick={() => setEditingDevice(null)}
                            style={{
                                padding: '8px 16px',
                                backgroundColor: '#f44336',
                                color: 'white',
                                border: 'none'
                            }}
                        >
                            Cancel
                        </button>
                    </div>
                </div>
            )}

            {/* Devices List */}
            {loading ? (
                <p>Loading devices...</p>
            ) : (
                Object.keys(groupedDevices).map((clientName) => (
                    <div key={clientName} style={{ marginBottom: '2rem' }}>
                        <h3>Client: {clientName}</h3>
                        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                            <thead>
                                <tr style={{ backgroundColor: '#f2f2f2' }}>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>DevEUI</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Name</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Location</th>
                                    <th style={{ padding: '12px', textAlign: 'left' }}>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {groupedDevices[clientName].map((device) => (
                                    <tr key={device.devEui} style={{ borderBottom: '1px solid #ddd' }}>
                                        <td style={{ padding: '12px' }}>{device.devEui}</td>
                                        <td style={{ padding: '12px' }}>{device.name}</td>
                                        <td style={{ padding: '12px' }}>
                                            {device.location?.country}, {device.location?.province}, {device.location?.district}
                                        </td>
                                        <td style={{ padding: '12px' }}>
                                            <button
                                                onClick={() => {
                                                    setEditingDevice(device);
                                                    setEditName(device.name);
                                                    setEditCountry(device.location?.country || '');
                                                    setEditProvince(device.location?.province || '');
                                                    setEditDistrict(device.location?.district || '');
                                                }}
                                                style={{
                                                    padding: '6px 12px',
                                                    backgroundColor: '#2196F3',
                                                    color: 'white',
                                                    border: 'none',
                                                    marginRight: '8px'
                                                }}
                                            >
                                                Edit
                                            </button>
                                            <button
                                                onClick={() => deleteDevice(device.devEui)}
                                                style={{
                                                    padding: '6px 12px',
                                                    backgroundColor: '#f44336',
                                                    color: 'white',
                                                    border: 'none'
                                                }}
                                            >
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