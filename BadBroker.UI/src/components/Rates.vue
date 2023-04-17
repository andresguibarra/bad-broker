<template>
  <div class="rates">
    <h1>Rates Calculator</h1>
    <div class="input-section">
      <label for="startDate">Start Date:</label>
      <VueDatePicker v-model="startDate" format="yyyy-MM-dd" auto-apply></VueDatePicker>
      <label for="endDate">End Date:</label>
      <VueDatePicker v-model="endDate" format="yyyy-MM-dd" auto-apply></VueDatePicker>
      <label for="initialMoney">Initial Money:</label>
      <input class="input-initial-money" type="number" v-model="initialMoney" placeholder="Enter initial money" />
      <button class="button-calculate" @click="fetchData">Calculate</button>
    </div>

    <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
    </div>

    <div v-if="result">
      <h2>Results</h2>
      <table class="results-table">
        <thead>
          <tr>
            <th>Buy Date</th>
            <th>Sell Date</th>
            <th>Tool</th>
            <th>Revenue</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>{{ result.buyDate }}</td>
            <td>{{ result.sellDate }}</td>
            <td>{{ result.tool }}</td>
            <td>{{ result.revenue?.toFixed(2) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
import axios from 'axios'
import VueDatePicker from '@vuepic/vue-datepicker'

interface Result {
  buyDate: string | null
  sellDate: string | null
  tool: string | null
  revenue: number
}

export default {
  name: 'Rates',
  components: {
    VueDatePicker
  },
  data() {
    return {
      startDate: null as Date | null,
      endDate: null as Date | null,
      initialMoney: null as number | null,
      result: null as Result | null,
      errorMessage: null as string | null
    }
  },
  methods: {
    async fetchData() {
      try {
        const response = await axios.get('https://localhost:7115/api/rates/best', {
          params: {
            startDate: this.startDate?.toISOString().split('T')[0],
            endDate: this.endDate?.toISOString().split('T')[0],
            moneyUsd: this.initialMoney
          }
        })
        this.errorMessage = null;
        this.result = response.data
      } catch (error: any) {
        console.error('Error fetching data:', error)
        this.errorMessage = error.response?.data?.title + '\n' + JSON.stringify(error.response?.data?.errors);
      }
    }
  }
}
</script>

<style scoped>
.rates {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.input-section {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-around;
  align-items: center;
  margin-bottom: 20px;
}

.input-section label {
  margin: 10px;
}

.input-section input {
  margin-bottom: 10px;
}

.results-table {
  border-collapse: collapse;
  width: 100%;
  max-width: 800px;
}

.results-table th,
.results-table td {
  border: 1px solid #ccc;
  padding: 8px;
  text-align: left;
}

.results-table th {
  background-color: #f2f2f2;
  font-weight: bold;
}

.results-table tbody tr:nth-child(even) {
  background-color: #f2f2f2;
}
.input-initial-money {
  border: 1px solid #ced4da;
  border-radius: 4px;
  padding: 8px 12px;
  font-size: 16px;
  color: #495057;
  width: 100%;
  box-sizing: border-box;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.input-initial-money:focus {
  border-color: #80bdff;
  outline: 0;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
.button-calculate {
  background-color: #007bff;
  color: #ffffff;
  border: none;
  border-radius: 4px;
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.button-calculate:hover {
  background-color: #0056b3;
}
</style>
