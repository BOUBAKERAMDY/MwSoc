
package com.letsgo.ws.generated;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para ItineraryDto complex type.
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.
 * 
 * <pre>
 * &lt;complexType name="ItineraryDto"&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="BikeMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestLat" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestLon" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestinationCity" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="EndStation" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="EndStationDetails" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}StationInfo" minOccurs="0"/&gt;
 *         &lt;element name="IsInterCity" type="{http://www.w3.org/2001/XMLSchema}boolean" minOccurs="0"/&gt;
 *         &lt;element name="IsWalkingOnly" type="{http://www.w3.org/2001/XMLSchema}boolean" minOccurs="0"/&gt;
 *         &lt;element name="Note" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="OriginCity" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="OriginLat" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="OriginLon" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="StartStation" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="StartStationDetails" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}StationInfo" minOccurs="0"/&gt;
 *         &lt;element name="TotalKm" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="TotalSeconds" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="WalkToEndMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="WalkToStartMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "ItineraryDto", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", propOrder = {
    "bikeMeters",
    "destLat",
    "destLon",
    "destinationCity",
    "endStation",
    "endStationDetails",
    "isInterCity",
    "isWalkingOnly",
    "note",
    "originCity",
    "originLat",
    "originLon",
    "startStation",
    "startStationDetails",
    "totalKm",
    "totalSeconds",
    "walkToEndMeters",
    "walkToStartMeters"
})
public class ItineraryDto {

    @XmlElement(name = "BikeMeters")
    protected Double bikeMeters;
    @XmlElement(name = "DestLat")
    protected Double destLat;
    @XmlElement(name = "DestLon")
    protected Double destLon;
    @XmlElementRef(name = "DestinationCity", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> destinationCity;
    @XmlElementRef(name = "EndStation", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> endStation;
    @XmlElementRef(name = "EndStationDetails", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<StationInfo> endStationDetails;
    @XmlElement(name = "IsInterCity")
    protected Boolean isInterCity;
    @XmlElement(name = "IsWalkingOnly")
    protected Boolean isWalkingOnly;
    @XmlElementRef(name = "Note", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> note;
    @XmlElementRef(name = "OriginCity", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> originCity;
    @XmlElement(name = "OriginLat")
    protected Double originLat;
    @XmlElement(name = "OriginLon")
    protected Double originLon;
    @XmlElementRef(name = "StartStation", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> startStation;
    @XmlElementRef(name = "StartStationDetails", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<StationInfo> startStationDetails;
    @XmlElement(name = "TotalKm")
    protected Double totalKm;
    @XmlElement(name = "TotalSeconds")
    protected Double totalSeconds;
    @XmlElement(name = "WalkToEndMeters")
    protected Double walkToEndMeters;
    @XmlElement(name = "WalkToStartMeters")
    protected Double walkToStartMeters;

    /**
     * Obtiene el valor de la propiedad bikeMeters.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getBikeMeters() {
        return bikeMeters;
    }

    /**
     * Define el valor de la propiedad bikeMeters.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setBikeMeters(Double value) {
        this.bikeMeters = value;
    }

    /**
     * Obtiene el valor de la propiedad destLat.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getDestLat() {
        return destLat;
    }

    /**
     * Define el valor de la propiedad destLat.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setDestLat(Double value) {
        this.destLat = value;
    }

    /**
     * Obtiene el valor de la propiedad destLon.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getDestLon() {
        return destLon;
    }

    /**
     * Define el valor de la propiedad destLon.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setDestLon(Double value) {
        this.destLon = value;
    }

    /**
     * Obtiene el valor de la propiedad destinationCity.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getDestinationCity() {
        return destinationCity;
    }

    /**
     * Define el valor de la propiedad destinationCity.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setDestinationCity(JAXBElement<String> value) {
        this.destinationCity = value;
    }

    /**
     * Obtiene el valor de la propiedad endStation.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getEndStation() {
        return endStation;
    }

    /**
     * Define el valor de la propiedad endStation.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setEndStation(JAXBElement<String> value) {
        this.endStation = value;
    }

    /**
     * Obtiene el valor de la propiedad endStationDetails.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public JAXBElement<StationInfo> getEndStationDetails() {
        return endStationDetails;
    }

    /**
     * Define el valor de la propiedad endStationDetails.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public void setEndStationDetails(JAXBElement<StationInfo> value) {
        this.endStationDetails = value;
    }

    /**
     * Obtiene el valor de la propiedad isInterCity.
     * 
     * @return
     *     possible object is
     *     {@link Boolean }
     *     
     */
    public Boolean isIsInterCity() {
        return isInterCity;
    }

    /**
     * Define el valor de la propiedad isInterCity.
     * 
     * @param value
     *     allowed object is
     *     {@link Boolean }
     *     
     */
    public void setIsInterCity(Boolean value) {
        this.isInterCity = value;
    }

    /**
     * Obtiene el valor de la propiedad isWalkingOnly.
     * 
     * @return
     *     possible object is
     *     {@link Boolean }
     *     
     */
    public Boolean isIsWalkingOnly() {
        return isWalkingOnly;
    }

    /**
     * Define el valor de la propiedad isWalkingOnly.
     * 
     * @param value
     *     allowed object is
     *     {@link Boolean }
     *     
     */
    public void setIsWalkingOnly(Boolean value) {
        this.isWalkingOnly = value;
    }

    /**
     * Obtiene el valor de la propiedad note.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getNote() {
        return note;
    }

    /**
     * Define el valor de la propiedad note.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setNote(JAXBElement<String> value) {
        this.note = value;
    }

    /**
     * Obtiene el valor de la propiedad originCity.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getOriginCity() {
        return originCity;
    }

    /**
     * Define el valor de la propiedad originCity.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setOriginCity(JAXBElement<String> value) {
        this.originCity = value;
    }

    /**
     * Obtiene el valor de la propiedad originLat.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getOriginLat() {
        return originLat;
    }

    /**
     * Define el valor de la propiedad originLat.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setOriginLat(Double value) {
        this.originLat = value;
    }

    /**
     * Obtiene el valor de la propiedad originLon.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getOriginLon() {
        return originLon;
    }

    /**
     * Define el valor de la propiedad originLon.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setOriginLon(Double value) {
        this.originLon = value;
    }

    /**
     * Obtiene el valor de la propiedad startStation.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getStartStation() {
        return startStation;
    }

    /**
     * Define el valor de la propiedad startStation.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setStartStation(JAXBElement<String> value) {
        this.startStation = value;
    }

    /**
     * Obtiene el valor de la propiedad startStationDetails.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public JAXBElement<StationInfo> getStartStationDetails() {
        return startStationDetails;
    }

    /**
     * Define el valor de la propiedad startStationDetails.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public void setStartStationDetails(JAXBElement<StationInfo> value) {
        this.startStationDetails = value;
    }

    /**
     * Obtiene el valor de la propiedad totalKm.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getTotalKm() {
        return totalKm;
    }

    /**
     * Define el valor de la propiedad totalKm.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setTotalKm(Double value) {
        this.totalKm = value;
    }

    /**
     * Obtiene el valor de la propiedad totalSeconds.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getTotalSeconds() {
        return totalSeconds;
    }

    /**
     * Define el valor de la propiedad totalSeconds.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setTotalSeconds(Double value) {
        this.totalSeconds = value;
    }

    /**
     * Obtiene el valor de la propiedad walkToEndMeters.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getWalkToEndMeters() {
        return walkToEndMeters;
    }

    /**
     * Define el valor de la propiedad walkToEndMeters.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setWalkToEndMeters(Double value) {
        this.walkToEndMeters = value;
    }

    /**
     * Obtiene el valor de la propiedad walkToStartMeters.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getWalkToStartMeters() {
        return walkToStartMeters;
    }

    /**
     * Define el valor de la propiedad walkToStartMeters.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setWalkToStartMeters(Double value) {
        this.walkToStartMeters = value;
    }

}
