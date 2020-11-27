import React from "react";
import { BrowserRouter, Link, Route } from "react-router-dom";
import { SeverityLevel } from "@microsoft/applicationinsights-web";
import { getAppInsights } from "./TelemetryService";
import TelemetryProvider from "./telemetry-provider";
import "./App.css";

const Students = () => {
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState(false);
  const [data, setData] = React.useState([]);
  const [tableView, setTableView] = React.useState(true);

  function switchToTableView() {
    setError(false);
    setTableView(true);
    appInsights.trackEvent({ name: "StudentsView_Table" });
  }

  function switchToGridView() {
    setError(false);
    setTableView(false);
    appInsights.trackEvent({ name: "StudentsView_Grid" });
  }

  function switchToListView() {
    let foo = {
      field: { bar: "value" },
    };
    // This will crash the app; the error will show up in the Azure Portal
    return foo.fielld.bar;
  }


  React.useEffect(() => {
    setTraceId();
    setLoading(true);
    fetch("https://localhost:44333/students")
      .then((response) => response.json())
      .then((data) => {
        setError()
        setLoading(false);
        setData(data);
        appInsights.trackTrace({
          message: `Client fetched ${data.length} students from API`,
          severityLevel: SeverityLevel.Information,
        });
      })
      .catch((e) => {
        setLoading(false);
        setError('fetch failed');
      });
  }, []);


  if (loading) {
    return <div className="spinner-border" role="status">
    <span className="sr-only">Loading...</span>
  </div>
  }

  return <div>
    <h2>Students</h2>
    <button type="button" className="btn btn-sm" onClick={switchToTableView}>Table</button> | 
    <button type="button" className="btn btn-sm" onClick={switchToGridView}>Grid</button> | 
    <button type="button" className="btn btn-sm" onClick={switchToListView}>List</button>
    <br></br>
    <br></br>
    {
    error ?
    <div className="alert alert-danger" role="alert">
      Something went wrong! (client-side)
    </div>
    :
    tableView ?
    <table className="table"><tbody>
        {data.map((element) => (
          <tr key={element.id}><td><img height="32" src={element.avatar} alt="avt" ></img></td><td>{element.first_name} {element.last_name}</td><td>{element.email}</td></tr>
        ))}
      </tbody></table>
      :
      <div className="row">
        {data.map((element) => (
          <div key={element.id} className="col-sm-4 col-lg-2">
            <div className="card">
              <img className="card-img-top" src={element.avatar} alt="avt" ></img>
              <div className="card-body">
                {element.first_name} {element.last_name}<br/>{element.email}
              </div>
            </div>
          </div>
        ))}
        </div>
    }
  </div>
};

const Teachers = () => {
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState('');
  const [data, setData] = React.useState([]);

  React.useEffect(() => {
    setTraceId();
    setLoading(true);
    fetch("https://localhost:44333/teachers")
      .then((response) => response.json())
      .then((data) => {
        setLoading(false);
        setData(data);
      })
      .catch((e) => {
        setLoading(false);
        setError('fetch failed');
      });
  }, []);

  if (loading) {
    return <div className="spinner-border" role="status">
    <span className="sr-only">Loading...</span>
  </div>
  }

  return <div>
    <h2>Teachers</h2>
    <br></br>
    <br></br>
    <table className="table"><tbody>
        {data.map((element) => (
          <tr key={element.id}><td><img height="32" src={element.avatar} alt="avt" ></img></td><td>{element.first_name} {element.last_name}</td><td>{element.email}</td></tr>
        ))}
      </tbody></table>
  </div>
};

const Courses = () => {
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState('');
  const [data, setData] = React.useState([]);



  React.useEffect(() => {
    setTraceId();
    setLoading(true);
    fetch("https://localhost:44333/courses")
      .then((response) => response.json())
      .then((data) => {
        setLoading(false);
        setData(data);
      })
      .catch((e) => {
        setLoading(false);
        setError('fetch failed');
      });
  }, []);

  if (loading) {
    return <div className="spinner-border" role="status">
    <span className="sr-only">Loading...</span>
  </div>
  }

  if (error) {
    return <div className="alert alert-danger" role="alert">
      Something went wrong! (server-side - traceId: {appInsights.context.telemetryTrace.traceID})
    </div>
  }

  return <div>
    <h2>Courses</h2>
    <br></br>
    <br></br>
    <table className="table"><tbody>
        {data.map((element) => (
          <tr key={element.id}><td><img height="32" src={element.avatar} alt="avt" ></img></td><td>{element.first_name} {element.last_name}</td><td>{element.email}</td></tr>
        ))}
      </tbody></table>
  </div>
};

const Header = () => (
  <nav className="navbar navbar-expand-lg navbar-light bg-light">
       <Link className="nav-link" to="/">Students</Link>
       <Link className="nav-link" to="/courses">Courses</Link>
       <Link className="nav-link" to="/teachers">Teachers</Link>
  </nav>
);

let appInsights = null;

const App = () => {

  return (
    <BrowserRouter>
      <TelemetryProvider
        instrumentationKey="YOURINSTRUMENTATIONKEYHERE!!"
        after={() => {
          appInsights = getAppInsights();
        }}
      >
          <Header />
          <div className="App">
            <Route exact path="/" component={Students} />
            <Route path="/teachers" component={Teachers} />
            <Route path="/courses" component={Courses} />
        </div>
      </TelemetryProvider>
    </BrowserRouter>
  );
};


function uuidv4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

function setTraceId() {
  // in a SPA you need to set the traceID yourself.
  // otherwise it only changes per page change
  var traceId = uuidv4();
  appInsights.context.telemetryTrace.traceID = traceId;
}


export default App;
