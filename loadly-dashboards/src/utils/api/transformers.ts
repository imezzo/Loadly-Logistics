import type { AxiosHeaders, AxiosRequestConfig } from "axios";
import { apiPaginationAdapter } from "@/types/response/pagination";

export type RequestAdapter = (
  data: AxiosRequestConfig["data"],
  headers: AxiosHeaders
) => AxiosRequestConfig["data"];

export type ResponseAdapter = (
  data: AxiosRequestConfig["data"]
) => AxiosRequestConfig["data"];

export function transformApiRequest(
  data: AxiosRequestConfig["data"],
  headers: AxiosHeaders,
  requestAdapter?: RequestAdapter
): string | FormData {
  if (data instanceof FormData) {
    return data;
  }

  if (!requestAdapter) {
    return JSON.stringify(data);
  }

  const adaptedData = requestAdapter(data, headers);

  return adaptedData instanceof FormData
    ? adaptedData
    : JSON.stringify(adaptedData);
}

export function transformApiResponse(
  rawData: unknown,
  responseAdapter?: ResponseAdapter
): Record<string, unknown> {
  const parsedData = rawData ? JSON.parse(rawData as string) : rawData;

  if (!responseAdapter || !parsedData?.success) {
    return parsedData;
  }

  const formattedData = parsedData.pagination
    ? {
      additional: parsedData.additional ?? null,
      data: parsedData.data,
      pagination: apiPaginationAdapter(parsedData.pagination),
    }
    : parsedData.data;

  return { ...parsedData, data: responseAdapter(formattedData) };
}
