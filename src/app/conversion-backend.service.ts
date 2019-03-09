import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable({
  providedIn: "root"
})
export class ConversionBackendService {
  // url = "server";
  constructor(private http: HttpClient) {}

  conversionToCSharptoJson(valueString) {
    console.log(valueString);

    const httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json; charset=utf-8"
      })
    };
    return this.http.post(
      "https://localhost:5001/api/values",
      { value: valueString },
      httpOptions
    );
  }
}
