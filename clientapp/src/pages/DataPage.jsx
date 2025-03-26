import React, { useEffect, useState } from 'react';
import Navbar from './Navbar';

const DataPage = () => {
    const [clients, setClients] = useState([]);
    const [devices, setDevices] = useState([]);
    const [filteredDevices, setFilteredDevices] = useState([]);
    const [selectedClient, setSelectedClient] = useState('');
    const [selectedDevEui, setSelectedDevEui] = useState('');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [dataResults, setDataResults] = useState({});
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchClientsAndDevices();
    }, []);

    const fetchClientsAndDevices = async () => {
        try {
            const [clientRes, deviceRes] = await Promise.all([
                fetch('http://localhost:5000/api/client', {
                    headers: { Authorization: `Bearer ${token}` }
                }),
                fetch('http://localhost:5000/api/device', {
                    headers: { Authorization: `Bearer ${token}` }
                })
            ]);

            const clientData = await clientRes.json();
            const deviceData = await deviceRes.json();
            setClients(clientData);
            setDevices(deviceData);
        } catch (err) {
            console.error('Failed to fetch dropdown options:', err);
        }
    };

    const parseDate = (isoString) => {
        try {
            return new Date(isoString);
        } catch {
            return new Date(NaN);
        }
    };

    const groupDataByDeviceAndDate = (dataList, currentFilteredDevices) => {
        const grouped = {};
        dataList.forEach((item) => {
            const parsedDate = parseDate(item.createdAt);
            if (isNaN(parsedDate)) return;

            const dateKey = parsedDate.toISOString().split('T')[0];

            if (!grouped[item.devEui]) {
                grouped[item.devEui] = {
                    deviceInfo: currentFilteredDevices.find(d => d.devEui === item.devEui) || {},
                    dataByDate: {}
                };
            }

            if (!grouped[item.devEui].dataByDate[dateKey]) {
                grouped[item.devEui].dataByDate[dateKey] = [];
            }

            grouped[item.devEui].dataByDate[dateKey].push(item);
        });
        return grouped;
    };

    const groupDataByDate = (dataList) => {
        const grouped = {};
        dataList.forEach((item) => {
            const parsedDate = parseDate(item.createdAt);
            if (isNaN(parsedDate)) return;

            const dateKey = parsedDate.toISOString().split('T')[0];
            if (!grouped[dateKey]) grouped[dateKey] = [];
            grouped[dateKey].push(item);
        });
        return grouped;
    };

    const handleClientChange = (clientName) => {
        setSelectedClient(clientName);
        setSelectedDevEui('');
        const client = clients.find((c) => c.name === clientName);
        if (client) {
            const clientDevices = devices.filter((d) => d.clientId === client.id);
            setFilteredDevices(clientDevices);
        } else {
            setFilteredDevices([]);
        }
    };

    const handleSearch = async () => {
        const client = clients.find((c) => c.name === selectedClient);
        if (!client) return alert('Client not found.');
        if (!startDate || !endDate) return alert('Please select date range.');

        setLoading(true);
        try {
            if (selectedDevEui) {
                const res = await fetch(
                    `http://localhost:5000/api/data/device-id/${selectedDevEui}?startDateTime=${startDate}T00:00:00Z&endDateTime=${endDate}T23:59:59Z`,
                    { headers: { Authorization: `Bearer ${token}` } }
                );
                const data = await res.json();
                setTotalCount(data.length);
                setDataResults(groupDataByDate(data));
            } else {
                const res = await fetch(
                    `http://localhost:5000/api/data/client-id/${client.id}?startDateTime=${startDate}T00:00:00Z&endDateTime=${endDate}T23:59:59Z`,
                    { headers: { Authorization: `Bearer ${token}` } }
                );
                const data = await res.json();
                setTotalCount(data.length);
                setDataResults(groupDataByDeviceAndDate(data, filteredDevices));
            }
        } catch (err) {
            console.error('Fetch error:', err);
            alert('Error fetching data');
        } finally {
            setLoading(false);
        }
    };

    const formatPayload = (payloadString) => {
        try {
            const payload = JSON.parse(payloadString);
            return JSON.stringify(payload, null, 2);
        } catch {
            return payloadString;
        }
    };

    const renderDeviceData = (deviceData) => {
        return Object.entries(deviceData.dataByDate).map(([date, entries]) => (
            <div key={date} style={{ 
                marginBottom: '1rem',
                padding: '1rem',
                backgroundColor: '#f5f5f5',
                borderRadius: '4px'
            }}>
                <h5>{date} ({entries.length} records)</h5>
                <ul style={{ listStyle: 'none', padding: 0 }}>
                    {entries.map((entry, idx) => (
                        <li key={idx} style={{ 
                            marginBottom: '0.5rem',
                            padding: '0.5rem',
                            backgroundColor: '#fff',
                            borderRadius: '4px'
                        }}>
                            <div style={{ marginBottom: '0.5rem' }}>
                                <strong>Time: </strong>
                                {parseDate(entry.createdAt).toLocaleTimeString()}
                            </div>
                            <details>
                                <summary style={{ cursor: 'pointer' }}>View Payload</summary>
                                <pre style={{ 
                                    backgroundColor: '#f0f0f0',
                                    padding: '0.5rem',
                                    borderRadius: '4px',
                                    overflowX: 'auto',
                                    marginTop: '0.5rem'
                                }}>
                                    {formatPayload(entry.payload)}
                                </pre>
                            </details>
                        </li>
                    ))}
                </ul>
            </div>
        ));
    };

    return (
        <div style={{ padding: '2rem', maxWidth: '1200px', margin: '0 auto' }}>
            <Navbar/>
            <h2 style={{ marginBottom: '1.5rem', color: '#333' }}>IoT Data Query</h2>

            <div style={{ 
                marginBottom: '1.5rem',
                padding: '1rem',
                backgroundColor: '#f9f9f9',
                borderRadius: '4px'
            }}>
                <div style={{ marginBottom: '1rem' }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>Client Name:</label>
                    <select 
                        value={selectedClient} 
                        onChange={(e) => handleClientChange(e.target.value)}
                        style={{ 
                            width: '100%',
                            padding: '8px',
                            borderRadius: '4px',
                            border: '1px solid #ddd'
                        }}
                    >
                        <option value="">-- Select Client --</option>
                        {clients.map((client) => (
                            <option key={client.id} value={client.name}>
                                {client.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div style={{ marginBottom: '1rem' }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>DevEUI:</label>
                    <select
                        value={selectedDevEui}
                        onChange={(e) => setSelectedDevEui(e.target.value)}
                        disabled={!selectedClient}
                        style={{ 
                            width: '100%',
                            padding: '8px',
                            borderRadius: '4px',
                            border: '1px solid #ddd'
                        }}
                    >
                        <option value="">-- All Devices --</option>
                        {filteredDevices.map((d) => (
                            <option key={d.devEui} value={d.devEui}>
                                {d.devEui} ({d.name || 'No name'})
                            </option>
                        ))}
                    </select>
                </div>

                <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                    <div style={{ flex: 1 }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>Start Date:</label>
                        <input 
                            type="date" 
                            value={startDate} 
                            onChange={(e) => setStartDate(e.target.value)}
                            style={{ 
                                width: '100%',
                                padding: '8px',
                                borderRadius: '4px',
                                border: '1px solid #ddd'
                            }}
                        />
                    </div>
                    <div style={{ flex: 1 }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>End Date:</label>
                        <input 
                            type="date" 
                            value={endDate} 
                            onChange={(e) => setEndDate(e.target.value)}
                            style={{ 
                                width: '100%',
                                padding: '8px',
                                borderRadius: '4px',
                                border: '1px solid #ddd'
                            }}
                        />
                    </div>
                </div>

                <button 
                    onClick={handleSearch}
                    style={{ 
                        padding: '10px 16px',
                        backgroundColor: '#4CAF50',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer',
                        fontWeight: 'bold',
                        width: '100%'
                    }}
                    disabled={loading}
                >
                    {loading ? 'Searchi>ng...' : 'Search Data'}
                </button>
            </div>

            {loading ? (
                <div style={{ textAlign: 'center', padding: '2rem' }}>
                    <p>Loading data...</p>
                </div>
            ) : (
                <>
                    {totalCount > 0 && (
                        <div style={{ 
                            marginBottom: '1rem',
                            padding: '0.5rem',
                            backgroundColor: '#e8f5e9',
                            borderRadius: '4px',
                            textAlign: 'center'
                        }}>
                            <strong>Total Records Found: {totalCount}</strong>
                        </div>
                    )}

                    {Object.keys(dataResults).length > 0 && (
                        <div style={{ marginTop: '1.5rem' }}>
                            {selectedDevEui ? (
                                // Display for single device (grouped by date)
                                Object.entries(dataResults).map(([date, entries]) => (
                                    <div key={date} style={{ 
                                        marginBottom: '1.5rem',
                                        border: '1px solid #e0e0e0',
                                        borderRadius: '4px'
                                    }}>
                                        <div style={{ 
                                            padding: '0.75rem 1rem',
                                            backgroundColor: '#f5f5f5',
                                            borderBottom: '1px solid #e0e0e0'
                                        }}>
                                            <h4 style={{ margin: 0 }}>{date} ({entries.length} records)</h4>
                                        </div>
                                        <ul style={{ listStyle: 'none', padding: 0 }}>
                                            {entries.map((entry, idx) => (
                                                <li key={idx} style={{ 
                                                    padding: '1rem',
                                                    borderBottom: '1px solid #f0f0f0'
                                                }}>
                                                    <div style={{ marginBottom: '0.5rem' }}>
                                                        <strong>Time: </strong>
                                                        {parseDate(entry.createdAt).toLocaleTimeString()}
                                                    </div>
                                                    <details>
                                                        <summary style={{ cursor: 'pointer', fontWeight: 'bold' }}>View Payload Details</summary>
                                                        <pre style={{ 
                                                            backgroundColor: '#fafafa',
                                                            padding: '1rem',
                                                            borderRadius: '4px',
                                                            overflowX: 'auto',
                                                            marginTop: '0.5rem'
                                                        }}>
                                                            {formatPayload(entry.payload)}
                                                        </pre>
                                                    </details>
                                                </li>
                                            ))}
                                        </ul>
                                    </div>
                                ))
                            ) : (
                                // Display for all devices (grouped by device then date)
                                Object.entries(dataResults).map(([devEui, deviceData]) => (
                                    <div key={devEui} style={{ 
                                        marginBottom: '2rem',
                                        border: '1px solid #e0e0e0',
                                        borderRadius: '4px'
                                    }}>
                                        <div style={{ 
                                            padding: '0.75rem 1rem',
                                            backgroundColor: '#e3f2fd',
                                            borderBottom: '1px solid #e0e0e0'
                                        }}>
                                            <h3 style={{ margin: 0 }}>
                                                Device: {devEui}
                                                {deviceData.deviceInfo.name && ` (${deviceData.deviceInfo.name})`}
                                            </h3>
                                        </div>
                                        <div style={{ padding: '0.5rem 1rem' }}>
                                            {renderDeviceData(deviceData)}
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default DataPage;