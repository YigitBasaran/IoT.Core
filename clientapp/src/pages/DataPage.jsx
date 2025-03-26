import React, { useEffect, useState } from 'react';

const DataPage = () => {
    const [clients, setClients] = useState([]);
    const [devices, setDevices] = useState([]);
    const [selectedClient, setSelectedClient] = useState('');
    const [selectedDevEui, setSelectedDevEui] = useState('');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [dataResults, setDataResults] = useState([]);
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

    const groupDataByDate = (dataList) => {
        const grouped = {};
        dataList.forEach((item) => {
            const date = new Date(item.timestamp).toISOString().split('T')[0];
            if (!grouped[date]) grouped[date] = [];
            grouped[date].push(item);
        });
        return grouped;
    };

    const groupDataByDevice = (dataList) => {
        const grouped = {};
        dataList.forEach((item) => {
            if (!grouped[item.devEui]) grouped[item.devEui] = [];
            grouped[item.devEui].push(item);
        });
        return grouped;
    };

    const handleSearchByClient = async () => {
        const client = clients.find((c) => c.name === selectedClient);
        if (!client) return alert('Client not found.');

        try {
            setLoading(true);
            const res = await fetch(
                `http://localhost:5000/api/data/client-id/${client.id}?startDateTime=${startDate}&endDateTime=${endDate}`,
                { headers: { Authorization: `Bearer ${token}` } }
            );
            const data = await res.json();
            setTotalCount(data.length);
            setDataResults(groupDataByDevice(data));
        } catch (err) {
            console.error('Failed to fetch data:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleSearchByDevice = async () => {
        if (!selectedDevEui) return alert('Select a device first.');

        try {
            setLoading(true);
            const res = await fetch(
                `http://localhost:5000/api/data/device-id/${selectedDevEui}?startDateTime=${startDate}&endDateTime=${endDate}`,
                { headers: { Authorization: `Bearer ${token}` } }
            );
            const data = await res.json();
            setTotalCount(data.length);
            setDataResults(groupDataByDate(data));
        } catch (err) {
            console.error('Failed to fetch data:', err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ padding: '2rem' }}>
            <h2>IoT Data Query</h2>

            <div style={{ marginBottom: '1rem' }}>
                <label>Client Name:</label>
                <select value={selectedClient} onChange={(e) => setSelectedClient(e.target.value)}>
                    <option value="">-- Select Client --</option>
                    {clients.map((client) => (
                        <option key={client.id} value={client.name}>
                            {client.name}
                        </option>
                    ))}
                </select>

                <label style={{ marginLeft: '20px' }}>DevEUI:</label>
                <select value={selectedDevEui} onChange={(e) => setSelectedDevEui(e.target.value)}>
                    <option value="">-- Select DevEUI --</option>
                    {devices.map((d) => (
                        <option key={d.devEui} value={d.devEui}>
                            {d.devEui}
                        </option>
                    ))}
                </select>
            </div>

            <div style={{ marginBottom: '1rem' }}>
                <label>Start Date:</label>
                <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
                <label style={{ marginLeft: '10px' }}>End Date:</label>
                <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
            </div>

            <div style={{ marginBottom: '1rem' }}>
                <button onClick={handleSearchByClient}>Search by Client</button>
                <button onClick={handleSearchByDevice} style={{ marginLeft: '10px' }}>
                    Search by Device
                </button>
            </div>

            {loading ? (
                <p>Loading...</p>
            ) : (
                <>
                    <p>Total Data Count: {totalCount}</p>
                    {typeof dataResults === 'object' &&
                        Object.entries(dataResults).map(([groupKey, entries]) => (
                            <div key={groupKey} style={{ marginBottom: '1rem' }}>
                                <h4>{groupKey} ({entries.length} records)</h4>
                                <ul>
                                    {entries.map((entry, idx) => (
                                        <li key={idx}>
                                            <span>{new Date(entry.timestamp).toLocaleString()}</span>
                                            <details>
                                                <summary>Payload</summary>
                                                <pre>{JSON.stringify(entry.payload, null, 2)}</pre>
                                            </details>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        ))}
                </>
            )}
        </div>
    );
};

export default DataPage;
