
package com.letsgo.ws.generated;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para anonymous complex type.
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.
 * 
 * <pre>
 * &lt;complexType&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="GetStationsResult" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}ArrayOfStationInfo" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "", propOrder = {
    "getStationsResult"
})
@XmlRootElement(name = "GetStationsResponse")
public class GetStationsResponse {

    @XmlElementRef(name = "GetStationsResult", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<ArrayOfStationInfo> getStationsResult;

    /**
     * Obtiene el valor de la propiedad getStationsResult.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link ArrayOfStationInfo }{@code >}
     *     
     */
    public JAXBElement<ArrayOfStationInfo> getGetStationsResult() {
        return getStationsResult;
    }

    /**
     * Define el valor de la propiedad getStationsResult.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link ArrayOfStationInfo }{@code >}
     *     
     */
    public void setGetStationsResult(JAXBElement<ArrayOfStationInfo> value) {
        this.getStationsResult = value;
    }

}
