import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

@Injectable({
  providedIn: "root"
})
export class ConversionBackendService {
  url = "server";
  constructor(private http: HttpClient) {}
  conversionToCSharptoJson(valueString) {
    return this.http.get("https://localhost:5000/api/values");
  }
}
