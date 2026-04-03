# Network Devices Configuration Analyzer

A single-page web tool to parse and analyze Cisco-style network configurations (IOS / IOS-XE / NX-OS), then present results in searchable tables, visual maps, audits, and version inventory.

## What This Tool Does

- Parses raw router/switch configuration text from multiple devices
- Builds VLAN, interface, and static-route views
- Maps VLAN-to-router relationships and IP reachability
- Shows visual draggable relationship graphs
- Audits credential and security posture issues
- Collects and summarizes device version evidence
- Supports tab-scoped export and print

## Tab Guide with Screenshots

All screenshots below are captured from the live tool interface using the included sample configs:
- `sample-switch.cfg` (Core Switch with 10 VLANs)
- `sample-router.cfg` (Edge Router with 4 VRFs, BGP/OSPF)

---

### 📈 Overview
**Quick glance at all parsed devices and their core inventory counts**

![Overview Screenshot](docs/images/Overview.jpg)

The Overview tab displays a summary card for each device showing:
- Device name and type (Switch/Router)
- VLAN count and interface count
- Route count and routing protocol status
- Quick reference for network topology validation

---

### 📍 VLANs
**Complete VLAN database with subnet assignments and gateway information**

![VLANs Screenshot](docs/images/VLANs.jpg)

Lists all VLANs found across devices with columns for:
- VLAN ID and name
- Subnet CIDR notation
- Gateway IPs and interfaces
- VRF assignment
- Reachability status across the network
- Direct "View" links to router path details

---

### ⚙ Interfaces
**Detailed interface specifications including IP addressing and VLAN assignments**

![Interfaces Screenshot](docs/images/Interfaces.jpg)

Comprehensive interface table showing:
- Interface name and device
- IP address and subnet mask
- VLAN tagging mode (access/trunk)
- VRF membership
- Administrative/operational status
- Port channel and VLAN memberships

---

### 🔄 Routes
**Static route listings with destination networks and next-hop details**

![Routes Screenshot](docs/images/Routes.jpg)

Routes tab displays all configured static routes with:
- Destination network (CIDR)
- Next-hop IP address
- Route metric/distance
- Associated VRF
- Source device
- Reachability verification

---

### 🧩 Relationship Map
**Table-based VLAN-to-router relationship mapping with comprehensive path information**

![Relationship Map Screenshot](docs/images/Relationship%20Map.jpg)

The Relationship Map (table view) provides detailed relationship analysis:
- Search by VLAN ID, name, or subnet
- Gateway IPs and Layer 3 interfaces per VLAN
- Complete router path with interface details
- VRF scope and reachability count
- Interactive "View" buttons for drill-down analysis

---

### 📈 Relationship Graph
**Visual, interactive graph with draggable nodes showing VLAN→Switch→Router→Subnet flow**

![Relationship Graph Screenshot](docs/images/Relationship%20Graph.jpg)

Dynamic visualization allowing:
- Drag nodes to reposition for clarity
- Visual path flow: VLAN → Switch Interface → Router Interface → Subnet
- Color-coded nodes by entity type
- Click nodes for detailed relationship information
- Perfect for architecture diagrams and troubleshooting

---

### 🔒 Credentials Audit
**Complete inventory of all usernames, enable passwords, and line access credentials with encoding analysis**

![Credentials Audit Screenshot](docs/images/Credentials%20Audit.jpg)

Security-focused credentials table showing:
- Device origin for each credential
- Credential type (username, enable, console/VTY line)
- Password encoding (plaintext, Type 5, Type 7, Type 8, Type 9)
- Reversibility indicators for Type 7 credentials
- Line numbers for config reference
- Summary counts by encoding type

---

### 🛡 Config Audit
**Automated security and compliance finding assessment with severity classification**

![Config Audit Screenshot](docs/images/Config%20Audit.jpg)

Comprehensive audit report with severity-based classification:
- **HIGH** findings: plaintext credentials, missing enable secret, telnet enabled
- **MEDIUM** findings: weak encryption, incomplete hardening
- **LOW** findings: missing best practices, optional security features
- Severity summary counters at top
- Device-specific findings with clickable source line links
- Filterable by device, severity, and category

---

### 🧾 Version Collector
**Device OS version inventory with evidence extraction and platform hints**

![Version Collector Screenshot](docs/images/Version%20Collector.jpg)

Version management interface displaying:
- Device name and type classification
- OS version with color-coded version numbers
- Software platform hints (e.g., "iOS 15.2 - Catalyst 9300")
- Sample revision markers if present (v1.1)
- Evidence source tracking (sample-revision, comment-software, version-command)
- Summary counters: devices with versions, with software hints, with sample markers
- Searchable and fully exportable version inventory

---

### 🌐 Topology Map
**Network device and connection relationship visualization**

![Topology Map Screenshot](docs/images/Topology%20Map.jpg)

Visual representation of:
- Device interconnection topology
- Links between switches and routers
- Interface-level connection details
- Device roles and hierarchy
- Network architecture at a glance

---

### 🔁 Routing Protocols
**OSPF, BGP, and VRF-based dynamic routing configuration summary**

![Routing Protocols Screenshot](docs/images/Routing%20Protocols.jpg)

Detailed routing protocol analysis including:
- OSPF process IDs and area configurations
- BGP autonomous system numbers and neighbor details
- Route redistributions and metric adjustments
- VRF-specific routing instances
- Protocol status and configuration validation

---

### 🚀 VLAN → Router Map
**Direct correlation of VLAN gateway assignments to router interfaces and L3 services**

![VLAN → Router Map Screenshot](docs/images/VLAN-Router-Map.jpg)

Maps VLAN layer 3 services showing:
- Which VLAN gateway interfaces exist on which routers
- Subinterface and SVI configurations
- IP address assignments per VLAN/Router pair
- VRF membership for multi-tenant routing
- HSRP/redundancy details if configured

---

### 🌎 VRF Map
**Virtual Routing and Forwarding instance inventory with interface membership**

![VRF Map Screenshot](docs/images/VRF%20Map.jpg)

VRF configuration details including:
- VRF name and route distinguisher
- Interface memberships per VRF
- Routing protocol instances per VRF
- VRF-to-VLAN relationships
- Interoperability with global routing table

---

### 🛡 VLAN Segmentation
**Security-focused VLAN isolation verification and access control analysis**

![VLAN Segmentation Screenshot](docs/images/VLAN%20Segmentation.jpg)

Segmentation and security review showing:
- VLAN isolation status per device
- Access control list (ACL) applications
- VLAN access control entries (VACE)
- Inter-VLAN routing policies
- Trunk port analysis and allowed VLAN lists

---

### 🔍 VLAN Search
**Real-time filterable search interface for VLAN lookup by ID, name, or subnet**

![VLAN Search Screenshot](docs/images/VLAN%20Search.jpg)

Quick lookup tool providing:
- Search by VLAN ID, VLAN name, subnet, or gateway IP
- Instant results with matching VLAN details
- Direct links to related interfaces and routes
- Network planning and verification support

---

### 📍 IP Lookup
**Address space search and reachability verification across the network**

![IP Lookup Screenshot](docs/images/IP%20Lookup.jpg)

Address management interface for:
- Searching IP addresses or subnets across all interfaces
- Verifying address conflicts or duplicates
- Tracing interface ownership and device placement
- Planning new address allocations
- Documenting current IP utilization

## Key Features

### 1) Multi-device parsing
Load/paste multiple configs, each with a device name. The parser extracts:
- Device identity and type
- VLAN definitions and SVI/subinterface networks
- Interface metadata (IP, mode, VLAN, VRF, status)
- Static routes
- OSPF/BGP/VRF/ACL/trunk data
- Credentials and encoding type
- Version metadata and evidence lines

### 2) Relationship mapping
The tool correlates VLAN IDs, gateway interfaces, router paths, and reachable networks.
- Table mode for exact details
- Visual graph mode for path-style flow: VLAN -> Switch -> Router -> Subnet
- Graph nodes can be dragged by users

### 3) Credential extraction
Detects and classifies:
- Username credentials
- Enable credentials
- Console/VTY line passwords
- Encoding types: plaintext, type 7, type 5, type 8, type 9, etc.
- Decoding support for reversible type 7 only

### 4) Config Audit
Automated checks with severity levels:
- High, Medium, Low
- Examples: plaintext passwords, telnet enabled, missing hardening lines
- Findings include source line references

### 5) Version Collector
Per-device version inventory with evidence:
- OS version from config
- Software/platform hints
- Sample revision markers (if present)
- Boot image evidence (if present)
- Searchable/exportable table and summary counters

### 6) Export and Print
Works on the currently selected tab/category:
- Table tabs export CSV
- Card/visual tabs export text
- Print generates selected-tab-only output

## Included Sample Configs

- `sample-switch.cfg`
- `sample-router.cfg`

These files include enriched VLAN/VRF/routing/ACL/credential data for feature testing.

## How To Use

1. Open `index.html` in a browser.
2. Paste or load one or more device configs.
3. Click **Parse & Analyze**.
4. Navigate tabs by category.
5. Use search bars inside tabs for filtering.
6. Use **Export Selected** or **Print Selected** for current tab output.

## Input Expectations

Best results with Cisco-like syntax including examples such as:
- `hostname ...`
- `version ...`
- `interface ...`
- `ip address ...`
- `switchport ...`
- `ip route ...`
- `router ospf ...`
- `router bgp ...`
- `ip vrf ...` / `vrf definition ...`
- `username ...` / `enable ...` / `line vty ...`

## Notes & Limitations

- Type 7 credentials are reversible and can be decoded.
- One-way hashes (type 5/8/9) are identified but not decoded.
- Parsing is heuristic and optimized for common Cisco patterns.
- Very vendor-specific custom syntax may require parser extension.

## Project Files

- `index.html` - UI, rendering, visualizations, export/print logic
- `parser.js` - parsing and normalization logic
- `sample-switch.cfg` - sample core-switch config
- `sample-router.cfg` - sample edge-router config

## Future Improvement Ideas

- Compliance profiles (baseline/strict)
- More vendor syntax adapters
- Risk scoring dashboard per device
- Diff mode between two config snapshots
