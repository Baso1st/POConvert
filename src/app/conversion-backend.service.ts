import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: "root"
})
export class ConversionBackendService {
  url = "/server";
  constructor(private http: HttpClient) {}
  conversionToCSharptoJson(value) {
    // const body = JSON.stringify(value);
    // return this.http.post(this.url + "/conversion", body);
  }
}
